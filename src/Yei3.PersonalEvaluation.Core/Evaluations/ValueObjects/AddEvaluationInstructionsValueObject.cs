namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;

    public class AddEvaluationInstructionsValueObject : ValueObject<AddEvaluationInstructionsValueObject>
    {
        public long Id { get; set; }
        public string Instructions { get; set; }
    }
}