using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Binnacle
{
    public class ObjectiveBinnacle : FullAuditedEntity<long>
    {
        public virtual string Text { get; protected set; }
        public virtual long EvaluationMeasuredQuestionId { get; protected set; }
        [ForeignKey("EvaluationMeasuredQuestionId")]
        public virtual EvaluationMeasuredQuestion EvaluationMeasuredQuestion { get; protected set; }
    }
}