using System;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions
{
    public abstract class EvaluationQuestion : FullAuditedEntity<long>
    {
        protected EvaluationQuestion(long evaluationId, long evaluationQuestionId, DateTime terminationDateTime, EvaluationQuestionStatus status)
        {
            EvaluationId = evaluationId;
            EvaluationQuestionId = evaluationQuestionId;
            TerminationDateTime = terminationDateTime;
            Status = status;
        }

        public void SetAnswer(long questionId)
        {

        }

        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual Evaluation Evaluation { get; protected set; }
        public virtual long EvaluationQuestionId { get; protected set; }
        [ForeignKey("EvaluationQuestionId")]
        public virtual DateTime TerminationDateTime { get; protected set; }
        public virtual EvaluationQuestionStatus Status { get; set; }
        public virtual bool IsActive { get; protected set; }
    }
}