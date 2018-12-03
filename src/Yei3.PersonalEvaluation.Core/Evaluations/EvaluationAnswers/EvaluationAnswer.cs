using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers
{
    public class EvaluationAnswer : FullAuditedEntity<long>
    {
        public EvaluationAnswer(long evaluationQuestionId, long evaluationId)
        {
            EvaluationQuestionId = evaluationQuestionId;
            EvaluationId = evaluationId;
        }

        public virtual string Text { get; protected set; }
        public virtual long EvaluationQuestionId { get; protected set; }
        [ForeignKey("EvaluationQuestionId")]
        public virtual EvaluationQuestion EvaluationQuestion { get; protected set; }
        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual Evaluation Evaluation { get; protected set; }
    }
}