namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;
    using Question;

    public class QuestionValueObject : ValueObject<QuestionValueObject>
    {
        public string Text { get; set; }
        public QuestionType QuestionType { get; set; }
        public long? Id { get; set; }
        public long SectionId { get; set; }
    }
}