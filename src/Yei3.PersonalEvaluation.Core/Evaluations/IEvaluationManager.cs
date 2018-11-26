using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Yei3.PersonalEvaluation.Evaluations.Terms;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;

namespace Yei3.PersonalEvaluation.Evaluations
{

    public interface IEvaluationManager : IDomainService
    {
        Task<EvaluationTerm> GetUserNextEvaluationTermAsync();
        Task<ToDoesSummaryValueObject> GetUserToDoesSummary();
        Task<int> GetUserPendingAutoEvaluationsCountAsync();
        Task<List<EvaluationSummaryValueObject>> GetUserPendingAutoEvaluationsAsync();
        Task<List<RevisionSummaryValueObject>> GetUserPendingEvaluationRevisionsAsync();
        Task<int> GetUserPendingEvaluationsCountAsync();
        Task<int> GetUserPendingObjectivesCountAsync();
    }
}