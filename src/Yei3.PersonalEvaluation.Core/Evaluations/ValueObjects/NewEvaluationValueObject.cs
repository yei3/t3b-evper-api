namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;
    using Term;
    using Interfaces;

    public class NewEvaluationValueObject : ValueObject<NewEvaluationValueObject>, INamed, IDescribed
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
    }
}