namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Domain.Services;
    using System.Threading.Tasks;
    using ValueObjects;

    public interface IEvaluationManager : IDomainService
    {
        Task<long> CreateEvaluationAndGetIdAsync(NewEvaluationValueObject newEvaluationValueObject);
        Task<long> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveValueObject addEvaluationObjectiveValueObject);
        Task<long> AddEvaluationCapabilityAndGetIdAsync(AddEvaluationCapabilityValueObject addEvaluationCapabilityValueObject);
    }
}