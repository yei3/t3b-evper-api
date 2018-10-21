namespace Yei3.PersonalEvaluation.Evaluations.Capabilities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities.Auditing;
    using Interfaces;
    using Abp.Domain.Entities;
    using System.Collections.Generic;

    public class Capability : FullAuditedEntity<long>, IIndexed, IDescribed, IPassivable
    {
        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public  virtual Evaluation Evaluation { get; protected set; }

        public virtual byte Index { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }
        [ForeignKey("CapabilityId")]
        public virtual ICollection<UserCapability> UserCapabilities { get; protected set; }
    }
}