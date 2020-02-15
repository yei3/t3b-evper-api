using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Abp.UI;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.OrganizationUnits.Dto;
using Yei3.PersonalEvaluation.Roles.Dto;
using Yei3.PersonalEvaluation.Users.Dto;
using Yei3.PersonalEvaluation.OrganizationUnit;
using Abp.Collections.Extensions;
using System.Net.Mail;

namespace Yei3.PersonalEvaluation.Users
{
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailSender _emailSender;

        private readonly IRepository<AreaOrganizationUnit, long> _areaOrganizationUnitRepository;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IEmailSender emailSender,
            IRepository<AreaOrganizationUnit, long> areaOrganizationUnitRepository
        )
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
            _areaOrganizationUnitRepository = areaOrganizationUnitRepository;
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }

        public override async Task<UserDto> Update(UserDto input)
        {
            CheckUpdatePermission();

            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return await Get(input);
        }

        public override async Task Delete(EntityDto<long> input)
        {
            try
            {
                var user = await _userManager.GetUserByIdAsync(input.Id);
                await _userManager.DeleteAsync(user);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public async Task<UserDto> GetUserByUsername(string employeeNumber)
        {
            var user = await Repository.FirstOrDefaultAsync(x => x.UserName == employeeNumber);

            if (user.IsNullOrDeleted())
            {
                throw new UserFriendlyException(404, $"El usuario no ha sido encontrado con ese número de empleado");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();

            return userDto;
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        public async Task UpdateScholarshipAndEmailAddress(UpdateUserDto updateUser)
        {
            User user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());

            user.EmailAddress = updateUser.EmailAddress;
            user.Scholarship = updateUser.Scholarship;
        }

        public async Task RecoverPassword(RecoverPasswordDto recoverPassword)
        {
            CurrentUnitOfWork.SetTenantId(1);

            User user;
            try
            {
                user = await _userManager.FindByEmployeeNumberAsync(recoverPassword.EmployeeNumber);
            }
            catch (InvalidOperationException)
            {
                throw new EntityNotFoundException(typeof(User), recoverPassword.EmployeeNumber);
            }

            if (user.EmailAddress != recoverPassword.EmailAddress)
            {
                throw new UserFriendlyException(501,
                    $"Email {recoverPassword.EmailAddress} no coincide con el email del usuario {recoverPassword.EmployeeNumber}");
            }

            string newPassword = $"{CreateRandomPassword(8)}_t3B";

            if ((await _userManager.ChangePasswordAsync(user, newPassword)).Succeeded)
            {
                user.IsEmailConfirmed = false;

                MailMessage mail = new MailMessage(
                new MailAddress("comunicadosrh@t3b.com.mx", "Soporte Tiendas 3B"),
                new MailAddress(user.EmailAddress, user.FullName)
            );
                mail.Subject = "Soporte Tiendas 3B - Recuperación de contraseña";
                mail.Body = $"Su nueva contraseña es {newPassword}. Al iniciar debe cambiarla.";

                try
                {
                    await _emailSender.SendAsync(mail);
                }
                catch (Exception)
                {
                    throw new UserFriendlyException(501, $"Hubo un error al en enviar el email, tu nueva contraseña es: {newPassword}");
                }
            }
        }

        public async Task<ICollection<string>> GetAllJobDescriptions()
        {
            return await _userManager
                .Users
                .Select(user => user.JobDescription)
                .Where(jobDescription => !jobDescription.IsNullOrEmpty())
                .Distinct()
                .OrderBy(jobDescription => jobDescription)
                .ToListAsync();
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserExte MapToEntityDto(User user)
        {
            List<Abp.Organizations.OrganizationUnit> organizationUnits = _userManager.GetOrganizationUnitsAsync(user).GetAwaiter().GetResult();

            var userDto = base.MapToEntityDto(user);
            userDto.OrganizationUnits = organizationUnits.MapTo<List<OrganizationUnitDto>>();

            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public override Task<PagedResultDto<UserDto>> GetAll(PagedResultRequestDto input)
        {
            return base.GetAll(input);
        }

        private static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public async Task<ICollection<User>> GetCollaborators()
        {
            User supervisorUser = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());
            bool isSupervisor = await _userManager.IsInRoleAsync(supervisorUser, StaticRoleNames.Tenants.Supervisor);
            if (!isSupervisor)
            {
                throw new UserFriendlyException($"El usuario {supervisorUser.FullName} no autorizado.");
            }
            List<User> users = _userManager
                .Users
                .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                .ToList();
            return users;
        }

        public async Task<bool> IsUserSalesMan()
        {
            User currentUser = await _userManager
                .Users
                .SingleAsync(user => user.Id == AbpSession.GetUserId());
            return await _userManager.IsUserASalesMan(currentUser);
        }

        public async Task<ICollection<UserAreaDto>> GetUsersByArea(long? areaId)
        {
            List<UserAreaDto> usersArea = new List<UserAreaDto>();

            IEnumerable<AreaOrganizationUnit> areas = _areaOrganizationUnitRepository
                .GetAll()
                .WhereIf(areaId.HasValue, area => area.Id == areaId.Value);

            foreach (AreaOrganizationUnit area in areas)
            {
                IEnumerable<UserAreaDto> users = (await _userManager
                    .GetUsersInOrganizationUnit(area))
                    .Select(user => new UserAreaDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        JobDescription = user.JobDescription,
                        AreaId = area.Id
                    });

                usersArea.AddRange(users);
            }

            return usersArea;
        }
        public async Task<ICollection<UserFullNameDto>> GetSubordinatesByUser(long userId)
        {
            User currentUser;
            try
            {
                currentUser = await _userManager.GetUserByIdAsync(userId);
            }
            catch (Exception)
            {
                throw new EntityNotFoundException(typeof(User), userId);
            }
            return (await _userManager.GetSubordinates(currentUser))
                .Where(user => !user.JobDescription.IsNullOrEmpty())
                .MapTo<ICollection<UserFullNameDto>>();
        }
    }
}
