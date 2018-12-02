using Yei3.PersonalEvaluation.Dashboard.Dto;

namespace Yei3.PersonalEvaluation.Dashboard
{
    using System.Threading.Tasks;
    using Abp.Application.Services;

    public interface IDashboardAppService : IApplicationService
    {
        Task<CollaboratorUserDashboardDto> Collaborator();
        Task<SupervisorUserDashboardDto> Supervisor();

    }
}