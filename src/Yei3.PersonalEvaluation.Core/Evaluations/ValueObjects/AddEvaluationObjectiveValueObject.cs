using Yei3.PersonalEvaluation.Interfaces;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;

    public class AddEvaluationObjectiveValueObject : ValueObject<AddEvaluationObjectiveValueObject>, IDescribed
    {
        public long EvaluationId { get; set; }
        public byte Index { get; set; }
        public string Description { get; set; }
    }
}