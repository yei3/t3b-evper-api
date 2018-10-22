namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;
    using Term;

    public class NewEvaluationValueObject : ValueObject<NewEvaluationValueObject>
    {
        public long EvaluatorUserId { get; set; }
        public long EvaluatedUserId { get; set; }
        public EvaluationTerm Term { get; set; }
    }
}