using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Core.OrganizationUnit;
using Yei3.PersonalEvaluation.MultiTenancy;
using Yei3.PersonalEvaluation.OrganizationUnit;

namespace Yei3.PersonalEvaluation.Authorization.Users
{
    public class UserRegistrationManager : DomainService
    {
        public IAbpSession AbpSession { get; set; }

        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;
        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> _organizationUnitRepository;

        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        private readonly string PasswordSalt = "_t3B";

        public UserRegistrationManager (
            TenantManager tenantManager,
            UserManager userManager,
            RoleManager roleManager,
            IPermissionManager permissionManager,
            IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository)
        {
            _tenantManager = tenantManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionManager = permissionManager;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            AbpSession = NullAbpSession.Instance;
        }

        public async Task<User> RegisterAsync (string name, string surname, string emailAddress, string userName, string plainPassword, bool isEmailConfirmed)
        {
            CheckForTenant();

            var tenant = await GetActiveTenantAsync();

            var user = new User {
                TenantId = tenant.Id,
                Name = name,
                Surname = surname,
                EmailAddress = emailAddress,
                IsActive = true,
                UserName = userName,
                IsEmailConfirmed = isEmailConfirmed,
                Roles = new List<UserRole> ()
            };

            user.SetNormalizedNames ();

            foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
            {
                user.Roles.Add(new UserRole(tenant.Id, user.Id, defaultRole.Id));
            }

            await _userManager.InitializeOptionsAsync(tenant.Id);

            CheckErrors(await _userManager.CreateAsync(user, plainPassword));
            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> ImportUserAsync (
            string employeeNumber,
            bool status,
            string firstLastName,
            string secondLastName,
            string name,
            string jobDescription,
            string area,
            string region,
            string immediateSupervisor,
            string socialReason,
            bool isManager,
            bool isSupervisor,
            string entryDate,
            string reassignDate,
            string birthDate,
            string scholarship,
            string email,
            bool isMale,
            bool isSalesArea)
        {
            using (IUnitOfWorkCompleteHandle unitOfWork = UnitOfWorkManager.Begin())
            {

                UnitOfWorkManager.Current.SetTenantId(1);
                UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete);

                User user = new User {
                    EmployeeNumber = employeeNumber,
                    UserName = employeeNumber,
                    IsActive = status,
                    Surname = $"{firstLastName} {secondLastName}",
                    Name = name,
                    JobDescription = jobDescription,
                    Area = area,
                    Region = region,
                    ImmediateSupervisor = immediateSupervisor,
                    SocialReason = socialReason,
                    EntryDate = NormalizeDateTime (entryDate),
                    ReassignDate = NormalizeDateTime (reassignDate),
                    BirthDate = NormalizeDateTime (birthDate),
                    Scholarship = scholarship,
                    IsEmailConfirmed = false,
                    TenantId = 1,
                    Roles = new List<UserRole> (),
                    EmailAddress = email,
                    IsMale = isMale
                };

                var organizationUnit = _organizationUnitRepository
                    .GetAll()
                    .Where(ou => ou.Parent.DisplayName == user.Region)
                    .LastOrDefault(ou => ou.DisplayName == user.Area);

                //* Create Organization Unit if not exists
                if (organizationUnit.IsNullOrDeleted())
                {
                    organizationUnit = await CreateOrganizationUnits(organizationUnit, user, isSalesArea);
                }

                try
                {
                    //* mostly cause email is not set or repeated
                    user.EmailAddress = $"{user.UserName}@tiendas3b.com";
                    await _userManager.CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.EmailAddress);
                }
                catch (UserFriendlyException)
                {
                    //! This must be implement on a better way or is the only one?
                    User existingUser = await _userManager.FindByEmployeeNumberAsync(user.EmployeeNumber);

                    if (!status) {
                        await _userManager.DeleteAsync(existingUser);
                        return user;
                    }

                    // Punto 4 PBI 1019
                    var userOrganizationUnit = _userOrganizationUnitRepository.FirstOrDefault(
                        uou => uou.UserId == existingUser.Id
                    );

                    if (userOrganizationUnit.IsNullOrDeleted())
                    {
                        if (userOrganizationUnit == null) 
                            await _userManager.AddToOrganizationUnitAsync(existingUser, organizationUnit);
                        else 
                            userOrganizationUnit.UnDelete();
                    }
                    else if (existingUser.Area != user.Area)
                    {
                        userOrganizationUnit.OrganizationUnitId = organizationUnit.Id;
                        await _userManager.SetOrganizationUnitsAsync(existingUser.Id, organizationUnit.Id);
                    }

                    //* Adding excel data to existing user                    
                    existingUser.JobDescription = user.JobDescription;
                    existingUser.Area = user.Area;
                    existingUser.Region = user.Region;
                    existingUser.ImmediateSupervisor = user.ImmediateSupervisor;
                    existingUser.SocialReason = user.SocialReason;
                    existingUser.ReassignDate = user.ReassignDate;
                    existingUser.Scholarship = user.Scholarship;
                    existingUser.EmailAddress = email.IsNullOrEmpty() ? $"{user.UserName}@tiendas3b.com" : email;

                    //! Just for deleted users
                    if (existingUser.IsDeleted)
                    {
                        existingUser.IsDeleted = false;
                        existingUser.DeleterUser = null;
                        existingUser.DeletionTime = null;
                        existingUser.LastModificationTime = DateTime.Now;
                        existingUser.LastModifierUserId = AbpSession.UserId;

                        await AddOrRemoveUserRole(existingUser, isSupervisor, isManager);

                        await _userManager.UpdateAsync(existingUser);

                        await _userManager.ChangePasswordAsync(existingUser, $"{existingUser.EmployeeNumber}_t3B");

                        await unitOfWork.CompleteAsync();

                        return existingUser;
                    }

                    await AddOrRemoveUserRole(existingUser, isSupervisor, isManager);

                    await _userManager.UpdateAsync(existingUser);

                    await unitOfWork.CompleteAsync();

                    return existingUser;
                }

                //* Create User if not exists
                user.EmailAddress = email.IsNullOrEmpty() ? $"{user.UserName}@tiendas3b.com" : email;
                CheckErrors(await _userManager.CreateAsync(user, $"{user.EmployeeNumber}{PasswordSalt}"));

                await AddOrRemoveUserRole(user, isSupervisor, isManager);

                await _userManager.AddToOrganizationUnitAsync(user, organizationUnit);

                await unitOfWork.CompleteAsync();

                return user;
            }
        }

        public async Task AddOrRemoveUserRole(User user, bool isSupervisor, bool isManager)
        {
            if (isManager)
            {
                await _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Administrator);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, StaticRoleNames.Tenants.Administrator);
            }

            if (isSupervisor)
            {
                await _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Supervisor);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, StaticRoleNames.Tenants.Supervisor);
            }

            await _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Collaborator);
        }

        public async Task<Abp.Organizations.OrganizationUnit> CreateOrganizationUnits(Abp.Organizations.OrganizationUnit area, User user, bool isSalesArea)
        {
            var region = _organizationUnitRepository
                    .GetAll()
                    .FirstOrDefault(ou => ou.DisplayName == user.Region);

            // Create region if not exists
            if (region.IsNullOrDeleted())
            {
                region = await _organizationUnitRepository.InsertAsync( 
                    new RegionOrganizationUnit(CurrentUnitOfWork.GetTenantId().Value, user.Region));

                var lastRegion = await _organizationUnitRepository
                    .GetAll()
                    .Where(ou => ou.ParentId == null)
                    .LastOrDefaultAsync();

                region.Code = Abp.Organizations.OrganizationUnit.CalculateNextCode(lastRegion.Code);

                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Validate if a Sales Area Organization Unit
            if (!isSalesArea)
            {
                area = await _organizationUnitRepository.InsertAsync(
                    new AreaOrganizationUnit(CurrentUnitOfWork.GetTenantId().Value, user.Area, region.Id)
                );
            }
            else
            {
                area = await _organizationUnitRepository.InsertAsync(
                    new SalesAreaOrganizationUnit(CurrentUnitOfWork.GetTenantId().Value, user.Area, region.Id)
                );
            }

            var lastArea = await _organizationUnitRepository
                .GetAll()
                .Where(ou => ou.Parent.DisplayName == region.DisplayName)
                .LastOrDefaultAsync();

            area.Code = lastArea.IsNullOrDeleted() ? "00001" : Abp.Organizations.OrganizationUnit.CalculateNextCode(lastArea.Code);

            await CurrentUnitOfWork.SaveChangesAsync();

            return area;
        }

        public async Task<bool> IsFirstTimeLogin(string userNameOrEmailAddress)
        {
            User user = await _userManager.FindByEmployeeNumberAsync(userNameOrEmailAddress);
            return !(await _userManager.IsEmailConfirmedAsync(user));
        }

        private DateTime NormalizeDateTime(string dateString) // 07/02/2014
        {

            if (dateString.IsNullOrEmpty())
            {
                return new DateTime(0);
            }

            int firstSlashIndex = dateString.IndexOf('/');
            int secondSlashIndex = dateString.Substring(firstSlashIndex + 1).IndexOf('/') + firstSlashIndex;

            int days = int.Parse(dateString.Substring(0, firstSlashIndex));
            int months = int.Parse(dateString.Substring(firstSlashIndex + 1, secondSlashIndex - firstSlashIndex));
            int years = int.Parse(dateString.Substring(secondSlashIndex + 2));

            return new DateTime(years, months, days);
        }

        private string ParseRole(string role)
        {
            switch (role) {
                case "COLABORADOR":
                    return StaticRoleNames.Tenants.Collaborator;
                case "JEFE":
                    return StaticRoleNames.Tenants.Supervisor;
                case "ADMINISTRADOR":
                    return StaticRoleNames.Tenants.Administrator;
                default:
                    return string.Empty;
            }
        }

        private void CheckForTenant()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                throw new InvalidOperationException("Can not register host users!");
            }
        }

        private async Task<Tenant> GetActiveTenantAsync ()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return await GetActiveTenantAsync(AbpSession.TenantId.Value);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await _tenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L ("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L ("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}