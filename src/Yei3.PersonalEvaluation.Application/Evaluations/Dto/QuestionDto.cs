using JetBrains.Annotations;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.AutoMapper;
    using ValueObjects;

    using Question;
    [AutoMap(typeof(QuestionValueObject))]
    public class QuestionDto
    {
        public string Text { get; set; }
        public QuestionType QuestionType { get; set; }
        [CanBeNull]public long? Id { get; set; }
        public long SectionId { get; set; }
    }
}