namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;
    using Interfaces;

    public class AddEvaluationCapabilityValueObject : ValueObject<AddEvaluationCapabilityValueObject>, INamed, IDescribed, IIndexed
    {
        public long EvaluationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Index { get; set; }
    }
}