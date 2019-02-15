using System;
using Abp.Domain.Entities.Auditing;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers
{
    public abstract class Answer : FullAuditedEntity<long>
    {
        protected Answer(long evaluationQuestionId)
        {
            EvaluationQuestionId = evaluationQuestionId;
        }

        protected Answer()
        {
        }
        
        public virtual string Text { get; protected set; }
        public virtual string Action { get; protected set; }        
        public virtual DateTime CommitmentDate { get; protected set; }
        public virtual long EvaluationQuestionId { get; protected set; }
    }
}