using System;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions
{
    public abstract class EvaluationQuestion : FullAuditedEntity<long>
    {
        protected EvaluationQuestion()
        {
        }

        protected EvaluationQuestion(long evaluationId, DateTime terminationDateTime, EvaluationQuestionStatus status)
        {
            EvaluationId = evaluationId;
            TerminationDateTime = terminationDateTime;
            Status = status;
        }

        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual Evaluation Evaluation { get; protected set; }
        public virtual DateTime TerminationDateTime { get; protected set; }
        public virtual EvaluationQuestionStatus Status { get; set; }
        public virtual bool IsActive { get; protected set; }
    }
}