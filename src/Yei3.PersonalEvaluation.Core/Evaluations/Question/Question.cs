namespace Yei3.PersonalEvaluation.Evaluations.Question
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities.Auditing;
    public class Question : FullAuditedEntity<long>
    {
        public Question()
        {
        }

        public Question(string text, QuestionType questionType, long sectionId, long? id)
        {
            Text = text;
            QuestionType = questionType;
            SectionId = sectionId;
            Id = id ?? 0;
        }

        public string Text { get; protected set; }
        public QuestionType QuestionType { get; set; }
        public string Answer { get; protected set; }
        public long SectionId { get; protected set; }
        [ForeignKey("SectionId")]
        public Section.Section Section { get; protected set; }
    }
}