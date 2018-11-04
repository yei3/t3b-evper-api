namespace Yei3.PersonalEvaluation.Evaluations.Objectives
{
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;
    using Interfaces;

    public class EvaluationUserObjective : FullAuditedEntity<long>, IAccomplished
    {
        public virtual long EvaluationUserId { get; protected set; }
        [ForeignKey("EvaluationUserId")]
        public virtual EvaluationUser EvaluationUser { get; protected set; }
        public virtual long ObjectiveId { get; protected set; }
        [ForeignKey("ObjectiveId")]
        public virtual Objective Objective { get; protected set; }

        public virtual bool IsAccomplished { get; set; }
    }
}
