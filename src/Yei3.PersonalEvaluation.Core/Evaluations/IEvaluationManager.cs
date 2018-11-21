namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Domain.Services;
    using System.Threading.Tasks;
    using ValueObjects;
    using System.Collections.Generic;

    public interface IEvaluationManager : IDomainService
    {
        Task<long> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveValueObject addEvaluationObjectiveValueObject);
        Task<long> InsertOrUpdateSectionAndGetIdAsync(SectionValueObject addEvaluationSectionValueObject);
        Task<long> InsertOrUpdateSubsectionAndGetIdAsync(SubsectionValueObject addSubsectionValueObject);
        Task<long> InsertOrUpdateQuestionAndGetIdAsync(QuestionValueObject questionValueObject);

        Task<long> AddEvaluationInstructionsAndGetIdAsync(AddEvaluationInstructionsValueObject addEvaluationInstructionsValueObject);
        Task<ICollection<long>> EvaluateUsers(long evaluationId, ICollection<long> userIds);
    }
}