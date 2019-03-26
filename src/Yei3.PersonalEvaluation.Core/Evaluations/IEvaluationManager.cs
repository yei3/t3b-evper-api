using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Yei3.PersonalEvaluation.Evaluations.Terms;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;

namespace Yei3.PersonalEvaluation.Evaluations
{

    public interface IEvaluationManager : IDomainService
    {
        Task<EvaluationTerm> GetUserNextEvaluationTermAsync(long? userId = null);
        Task<ToDoesSummaryValueObject> GetUserToDoesSummary(long? userId = null);
        Task<int> GetUserPendingAutoEvaluationsCountAsync(long? userId = null);
        Task<List<EvaluationSummaryValueObject>> GetUserPendingEvaluationsAsync(long? userId = null);
        Task<List<RevisionSummaryValueObject>> GetUserPendingEvaluationRevisionsAsync(long? userId = null);
        Task<List<EvaluationObjectivesSummaryValueObject>> GetUserPendingObjectiveAsync(long? userId = null);
        Task<int> GetUserPendingEvaluationsCountAsync(long? userId = null);
        Task<int> GetUserPendingObjectivesCountAsync(long? userId = null);
        Task<int> GetUserOrganizationUnitPendingEvaluationsCountAsync(long? userId = null);
        Task<int> GetUserOrganizationUnitPendingEvaluationValidationsCountAsync(long? userId = null);

        Task<ICollection<EvaluationSummaryValueObject>> GetUserOrganizationUnitCollaboratorsPendingEvaluationsAsync(
            long? userId = null);
        Task<ICollection<RevisionSummaryValueObject>> GetUserOrganizationUnitPendingEvaluationRevisionsAsync(long? userId = null);
        Task<ICollection<CollaboratorsPendingObjectivesSummaryValueObject>> GetUserOrganizationUnitObjectivesSummaryAsync(long? userId = null);

        Task<ICollection<CollaboratorsPendingObjectivesSummaryValueObject>> GetUserOrganizationUnitObjectivesHistoryAsync(long? userId = null);

        Task<List<EvaluationObjectivesSummaryValueObject>> GetUserObjectivesHistoryAsync(long? userId = null);

    }
}