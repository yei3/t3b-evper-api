using System.Threading.Tasks;
using Abp.Application.Services;
using Yei3.PersonalEvaluation.Sessions.Dto;

namespace Yei3.PersonalEvaluation.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
