using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Yei3.PersonalEvaluation.Application.Authorization.Accounts.Dto;
using Yei3.PersonalEvaluation.Authorization.Accounts.Dto;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Authorization.Accounts
{

    using Abp.Authorization;

    public class AccountAppService : PersonalEvaluationAppServiceBase, IAccountAppService
    {
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UserManager _userManager;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
            UserManager userManager)
        {
            _userRegistrationManager = userRegistrationManager;
            _userManager = userManager;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        [AbpAuthorize]
        public async Task<RegisterOutput> FirstTimeLoginAsync(RegisterEmployeeInput input)
        {
            User user;

            try
            {
                user = await _userManager.FindByEmployeeNumberAsync(input.EmployeeNumber);
            }
            catch (InvalidOperationException)
            {
                Logger.Error($"Usuario {input.EmployeeNumber} no encontrado");
                return new RegisterOutput { CanLogin = false, HasErrors = true, Errors = new List<string> { $"Usuario {input.EmployeeNumber} no encontrado" } };
            }

            IdentityResult changePasswordIdentityResult = await _userManager.ChangePasswordAsync(user, input.Password);
            IdentityResult setEmailIdentityResult = await _userManager.SetEmailAsync(user, input.Email);

            RegisterOutput output = new RegisterOutput();

            if (!changePasswordIdentityResult.Succeeded || !setEmailIdentityResult.Succeeded)
            {
                output.HasErrors = true;
                output.CanLogin = false;

                output.Errors = changePasswordIdentityResult.Errors.Select(error => error.Description)
                    .Concat(setEmailIdentityResult.Errors.Select(error => error.Description)).ToList();
            }
            else
            {
                user.IsEmailConfirmed = true;
            }

            return output;
        }
    }
}
