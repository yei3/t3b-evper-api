using Abp.Domain.Entities.Auditing;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers
{
    public abstract class Answer : FullAuditedEntity<long>
    {
        protected Answer(long evaluationQuestionId)
        {
            EvaluationQuestionId = evaluationQuestionId;
        }

        public virtual string Text { get; protected set; }
        public virtual long EvaluationQuestionId { get; protected set; }
    }
}