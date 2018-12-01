using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Dashboard.Dto;
using Yei3.PersonalEvaluation.Evaluations;

namespace Yei3.PersonalEvaluation.Dashboard
{
    public class DashboardAppService : IDashboardAppService
    {
        private readonly EvaluationManager EvaluationManager;

        public DashboardAppService(EvaluationManager evaluationManager)
        {
            EvaluationManager = evaluationManager;
        }

        public async Task<CollaboratorUserDashboardDto> Collaborator()
        {
            return new CollaboratorUserDashboardDto
            {
                NextEvaluationTerm = await EvaluationManager.GetUserNextEvaluationTermAsync(),
                ToDoesSummary = (await EvaluationManager.GetUserToDoesSummary()).MapTo<ToDoesSummaryDto>(),
                AutoEvaluationSummary = (await EvaluationManager.GetUserPendingAutoEvaluationsAsync()).MapTo<ICollection<EvaluationSummaryDto>>(),
                RevisionSummary = (await EvaluationManager.GetUserPendingEvaluationRevisionsAsync()).MapTo<ICollection<RevisionSummaryDto>>(),
                ObjectiveSummary = (await EvaluationManager.GetUserPendingObjectiveAsync()).MapTo<ICollection<PendingObjectivesSummaryDto>>()
            };
        }
    }
}