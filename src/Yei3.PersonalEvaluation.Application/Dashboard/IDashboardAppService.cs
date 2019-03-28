using Yei3.PersonalEvaluation.Dashboard.Dto;

namespace Yei3.PersonalEvaluation.Dashboard
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Yei3.PersonalEvaluation.Evaluations.ValueObject;

    public interface IDashboardAppService : IApplicationService
    {
        Task<CollaboratorUserDashboardDto> Collaborator();
        Task<SupervisorUserDashboardDto> Supervisor();
        Task<CollaboratorUserDashboardDto> EvaluationsHistory();
        Task<SupervisorUserDashboardDto> SupervisorHistory();
        Task<CollaboratorUserDashboardDto> CollaboratorHistory();
        

    }
}