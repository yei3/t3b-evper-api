using System;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.Questions;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions
{
    public class EvaluationQuestion : FullAuditedEntity<long>
    {
        public EvaluationQuestion(long evaluationId, long questionId, DateTime terminationDateTime)
        {
            EvaluationId = evaluationId;
            QuestionId = questionId;
            TerminationDateTime = terminationDateTime;
        }

        public void SetAnswer(long evaluationQuestionId, long evaluationId)
        {
            Answer = new EvaluationAnswer(evaluationQuestionId, evaluationId);
        }

        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual Evaluation Evaluation { get; protected set; }
        public virtual long QuestionId { get; protected set; }
        [ForeignKey("QuestionId")]
        public virtual UnmeasuredQuestion UnmeasuredQuestion { get; protected set; }
        [ForeignKey("QuestionId")]
        public virtual MeasuredQuestion MeasuredQuestion { get; protected set; }
        [CanBeNull]
        [ForeignKey("QuestionId")]
        public virtual EvaluationAnswer Answer { get; protected set; }
        public virtual DateTime TerminationDateTime { get; protected set; }
        public virtual EvaluationQuestionStatus Status { get; protected set; }
    }
}