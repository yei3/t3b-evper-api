using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Abp.UI;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.MultiTenancy;
using Abp.Extensions;

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

        private readonly string PasswordSalt = "_t3B";

        public UserRegistrationManager(
            TenantManager tenantManager,
            UserManager userManager,
            RoleManager roleManager, IPermissionManager permissionManager, IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitRepository)
        {
            _tenantManager = tenantManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionManager = permissionManager;
            _organizationUnitRepository = organizationUnitRepository;

            AbpSession = NullAbpSession.Instance;
        }

        public async Task<User> RegisterAsync(string name, string surname, string emailAddress, string userName, string plainPassword, bool isEmailConfirmed)
        {
            CheckForTenant();

            var tenant = await GetActiveTenantAsync();

            var user = new User
            {
                TenantId = tenant.Id,
                Name = name,
                Surname = surname,
                EmailAddress = emailAddress,
                IsActive = true,
                UserName = userName,
                IsEmailConfirmed = isEmailConfirmed,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
            {
                user.Roles.Add(new UserRole(tenant.Id, user.Id, defaultRole.Id));
            }

            await _userManager.InitializeOptionsAsync(tenant.Id);

            CheckErrors(await _userManager.CreateAsync(user, plainPassword));
            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        public User ImportUser(
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
            bool isMale)
        {
            using (Abp.Domain.Uow.IUnitOfWorkCompleteHandle unitOfWork = UnitOfWorkManager.Begin())
            {

                _tenantManager.UnitOfWorkManager.Current.SetTenantId(1);

                User user = new User
                {
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
                    EntryDate = NormalizeDateTime(entryDate),
                    ReassignDate = NormalizeDateTime(reassignDate),
                    BirthDate = NormalizeDateTime(birthDate),
                    Scholarship = scholarship,
                    EmailAddress = email,
                    IsEmailConfirmed = false,
                    TenantId = 1,
                    Roles = new List<UserRole>(),
                    IsMale = isMale
                };


                try
                {
                    // mostly cause email is not set or repeated
                    user.EmailAddress = $"{user.UserName}@dummyemail.com";
                    _userManager.CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.EmailAddress).GetAwaiter().GetResult();
                }
                catch (UserFriendlyException e)
                {
                    User existingUser = _userManager.FindByEmployeeNumberAsync(user.EmployeeNumber).GetAwaiter().GetResult();
                    existingUser = user;
                    _userManager.UpdateAsync(existingUser).GetAwaiter().GetResult();
                    return existingUser;
                }

                _userManager.CreateAsync(user, $"{user.EmployeeNumber}{PasswordSalt}").GetAwaiter().GetResult();

                if (isManager)
                {
                    _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Administrator).GetAwaiter().GetResult();
                }

                if (isSupervisor)
                {
                    _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Supervisor).GetAwaiter().GetResult();
                }

                _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Collaborator).GetAwaiter().GetResult();

                var organizationUnit = _organizationUnitRepository
                    .GetAll()
                    .Where(ou => ou.Parent.DisplayName == user.Region)
                    .FirstOrDefault(ou => ou.DisplayName == user.Area);

                if (!organizationUnit.IsNullOrDeleted())
                {
                    _userManager.AddToOrganizationUnitAsync(user, organizationUnit).GetAwaiter().GetResult();
                }

                unitOfWork.Complete();

                return user;
            }
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
            switch (role)
            {
                case "COLABORADOR": return StaticRoleNames.Tenants.Collaborator;
                case "JEFE": return StaticRoleNames.Tenants.Supervisor;
                case "ADMINISTRADOR": return StaticRoleNames.Tenants.Administrator;
                default: return string.Empty;
            }
        }

        private void CheckForTenant()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                throw new InvalidOperationException("Can not register host users!");
            }
        }

        private async Task<Tenant> GetActiveTenantAsync()
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
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
