namespace Yei3.PersonalEvaluation.Evaluations.Capabilities
{
    using Abp.Domain.Entities.Auditing;
    using Interfaces;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EvaluationUserCapability : FullAuditedEntity<long>, IRated, ICommented
    {
        public virtual long EvaluationUserId { get; protected set; }
        [ForeignKey("EvaluationUserId")]
        public virtual EvaluationUser EvaluationUser { get; protected set; }
        public virtual long CapabilityId { get; protected set; }
        [ForeignKey("CapabilityId")]
        public virtual Capability Capability { get; protected set; }
        public virtual float Rate { get; set; }
        public virtual string Comment { get; set; }
    }
}