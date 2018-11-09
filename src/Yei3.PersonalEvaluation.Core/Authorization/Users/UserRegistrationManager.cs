using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization.Users;
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
        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly string PasswordSalt = "_t3B";

        public UserRegistrationManager(
            TenantManager tenantManager,
            UserManager userManager,
            RoleManager roleManager,
            IPasswordHasher<User> passwordHasher)
        {
            _tenantManager = tenantManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;

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
            string role,
            string entryDate,
            string reassignDate,
            string birthDate,
            string scholarship,
            string email)
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
                    Roles = new List<UserRole>()
                };


                try
                {
                    _userManager.CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.EmailAddress).GetAwaiter().GetResult();
                }
                catch (UserFriendlyException)
                {
                    // mostly cause email is not set or repeated
                    user.EmailAddress = $"{user.UserName}@dummyemail.com";
                }

                _userManager.CreateAsync(user, $"{user.EmployeeNumber}{PasswordSalt}").GetAwaiter().GetResult();

                switch (ParseRole(role))
                {
                    case StaticRoleNames.Tenants.Administrator:
                        {
                            _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Administrator).GetAwaiter().GetResult();
                            break;
                        }
                    case StaticRoleNames.Tenants.Supervisor:
                        {
                            _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Supervisor).GetAwaiter().GetResult();
                            break;
                        }
                }

                _userManager.AddToRoleAsync(user, StaticRoleNames.Tenants.Collaborator).GetAwaiter().GetResult();

                unitOfWork.Complete();

                return user;
            }
        }

        public async Task<bool> IsFirstTimeLogin(string userNameOrEmailAddress)
        {
            User user = await _userManager.FindByEmployeeNumberAsync(userNameOrEmailAddress);
            return !(await _userManager.IsEmailConfirmedAsync(user));
        }

        private DateTime NormalizeDateTime(string dateString)
        {

            if (dateString.IsNullOrEmpty())
            {
                return new DateTime(0);
            }

            int firstSlashIndex = dateString.IndexOf('/');
            int secondSlashIndex = dateString.Substring(firstSlashIndex + 1).IndexOf('/') + firstSlashIndex;
            int firstSpaceIndex = dateString.Substring(secondSlashIndex + 1).IndexOf(' ') + secondSlashIndex;

            int days = int.Parse(dateString.Substring(0, firstSlashIndex));
            int months = int.Parse(dateString.Substring(firstSlashIndex + 1, secondSlashIndex - firstSlashIndex));
            int years = int.Parse(dateString.Substring(secondSlashIndex + 2, firstSpaceIndex - secondSlashIndex));

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
