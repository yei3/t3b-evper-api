using System.Threading.Tasks;
using Abp.Application.Services;
using Yei3.PersonalEvaluation.Application.Authorization.Accounts.Dto;
using Yei3.PersonalEvaluation.Authorization.Accounts.Dto;

namespace Yei3.PersonalEvaluation.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);

        Task<RegisterOutput> FirstTimeLoginAsync(RegisterEmployeeInput input);
    }
}
