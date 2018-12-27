using System.Collections.Generic;
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
                EvaluationSummary = (await EvaluationManager.GetUserPendingAutoEvaluationsAsync()).MapTo<ICollection<EvaluationSummaryDto>>(),
                RevisionSummary = (await EvaluationManager.GetUserPendingEvaluationRevisionsAsync()).MapTo<ICollection<RevisionSummaryDto>>(),
                ObjectiveSummary = (await EvaluationManager.GetUserPendingObjectiveAsync()).MapTo<ICollection<PendingObjectivesSummaryDto>>()
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
                CollaboratorsEvaluationSummary = (await EvaluationManager.GetUserOrganizationUnitCollaboratorsPendingEvaluationsAsync()).MapTo<ICollection<EvaluationSummaryDto>>(),
                CollaboratorRevisionSummary = (await EvaluationManager.GetUserOrganizationUnitPendingEvaluationRevisionsAsync()).MapTo<ICollection<RevisionSummaryDto>>(),
                CollaboratorsObjectivesSummary = (await EvaluationManager.GetUserOrganizationUnitObjectivesSummaryAsync()).MapTo<ICollection<CollaboratorsObjectivesSummaryDto>>()
            };
        }
    }
}