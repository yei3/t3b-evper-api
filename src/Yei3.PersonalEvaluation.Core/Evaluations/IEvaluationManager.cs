using Yei3.PersonalEvaluation.Evaluations.ValueObjects;

namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Domain.Services;
    using System.Threading.Tasks;

    public interface IEvaluationManager : IDomainService
    {
        Task CreateEvaluation(NewEvaluationValueObject newEvaluationValueObject);
        Task AddEvaluationObjective(AddEvaluationObjectiveValueObject addEvaluationObjectiveValueObject);
    }
}