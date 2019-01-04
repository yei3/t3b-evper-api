using Abp.Domain.Entities.Auditing;

namespace Yei3.PersonalEvaluation.Evaluations.Questions
{
    public abstract class Question : FullAuditedEntity<long>
    {
        protected Question(string text, QuestionType questionType, bool isQualifiable)
        {
            Text = text;
            QuestionType = questionType;
            IsQualifiable = isQualifiable;
        }

        protected Question()
        {
        }

        public string Text { get; protected set; }
        public QuestionType QuestionType { get; set; }
        public virtual long SectionId { get; protected set; }
        public virtual bool IsQualifiable { get; protected set; }
    }
}