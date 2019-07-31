using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<CollaboratorUserDashboardDto> Collaborator()
        {
            return new CollaboratorUserDashboardDto
            {
                NextEvaluationTerm = await EvaluationManager.GetUserNextEvaluationTermAsync(),
                ToDoesSummary = (await EvaluationManager.GetUserToDoesSummary()).MapTo<ToDoesSummaryDto>(),
                EvaluationSummary = (await EvaluationManager.GetUserPendingEvaluationsAsync()).Where(evaluation => evaluation.IsAutoEvaluation).MapTo<ICollection<EvaluationSummaryDto>>(),
                RevisionSummary = (await EvaluationManager.GetUserPendingEvaluationRevisionsAsync()).MapTo<ICollection<RevisionSummaryDto>>(),
                ObjectiveSummary = (await EvaluationManager.GetUserObjectivesHome()).MapTo<ICollection<PendingObjectivesSummaryDto>>(),
                ActionSummary = (await EvaluationManager.GetUserActionsAsync()).MapTo<ICollection<EvaluationActionDto>>()
            };
        }

        [HttpGet]
        public async Task<SupervisorUserDashboardDto> Supervisor()
        {
            return new SupervisorUserDashboardDto
            {
                NextEvaluationTerm = await EvaluationManager.GetUserNextEvaluationTermAsync(),
                SupervisorToDoes = new SupervisorToDoes
                {
                    CollaboratorsObjectivesValidationPending = await EvaluationManager.GetUserOrganizationUnitPendingEvaluationValidationsCountAsync(),
                    CollaboratorsPendingEvaluations = await EvaluationManager.GetUserOrganizationUnitPendingEvaluationsCountAsync()
                },
                CollaboratorsEvaluationSummary = (await EvaluationManager.GetUserOrganizationUnitCollaboratorsPendingEvaluationsAsync()).Where(evaluation => !evaluation.IsAutoEvaluation).MapTo<ICollection<EvaluationSummaryDto>>(),
                CollaboratorRevisionSummary = (await EvaluationManager.GetUserOrganizationUnitPendingEvaluationRevisionsAsync()).MapTo<ICollection<RevisionSummaryDto>>(),
                CollaboratorsObjectivesSummary = (await EvaluationManager.GetUserOrganizationUnitObjectivesSummaryAsync()).MapTo<ICollection<CollaboratorsObjectivesSummaryDto>>(),
                ActionSummary = (await EvaluationManager.GetUserOrganizationUnitCollaboratorsActionsAsync()).MapTo<ICollection<EvaluationActionDto>>()
            };
        }

        [HttpGet]
        public async Task<CollaboratorUserDashboardDto> EvaluationsHistory()
        {
            return new CollaboratorUserDashboardDto
            {
                EvaluationSummary = (await EvaluationManager.GetUserEvaluationsHistoryAsync()).MapTo<ICollection<EvaluationSummaryDto>>(),                
            };
        }

        [HttpGet]
        public async Task<SupervisorUserDashboardDto> SupervisorHistory()
        {
            return new SupervisorUserDashboardDto
            {
                NextEvaluationTerm = await EvaluationManager.GetUserNextEvaluationTermAsync(),
                CollaboratorsEvaluationSummary = (await EvaluationManager.GetBossEvaluationsHistoryAsync()).MapTo<ICollection<EvaluationSummaryDto>>(),
                CollaboratorsObjectivesSummary = (await EvaluationManager.GetUserOrganizationUnitObjectivesHistoryAsync()).MapTo<ICollection<CollaboratorsObjectivesSummaryDto>>()
            };
        }

        [HttpGet]
        public async Task<CollaboratorUserDashboardDto> CollaboratorHistory()
        {
            return new CollaboratorUserDashboardDto
            {
                ObjectiveSummary = (await EvaluationManager.GetUserObjectivesHistory()).MapTo<ICollection<PendingObjectivesSummaryDto>>()
            };
        }
    }
}