using System;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Zero.Configuration;
using Yei3.PersonalEvaluation.Application.Authorization.Accounts.Dto;
using Yei3.PersonalEvaluation.Authorization.Accounts.Dto;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Authorization.Accounts
{
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

        public async Task<RegisterOutput> FirstTimeLoginAsync(RegisterEmployeeInput input)
        {
            User user;

            try
            {
                user = await _userManager.FindByEmployeeNumberAsync(input.EmployeeNumber);
            } catch(InvalidOperationException)
            {
                Logger.Error($"Usuario {input.EmployeeNumber} no encontrado");
                return new RegisterOutput { CanLogin = false };
            }

            await _userManager.ChangePasswordAsync(user, input.Password);
            await _userManager.SetEmailAsync(user, input.Email);

            return new RegisterOutput
            {
                CanLogin = true
            };
        }
    }
}
