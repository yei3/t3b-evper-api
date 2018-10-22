namespace Yei3.PersonalEvaluation.Evaluations.Capabilities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities.Auditing;
    using Interfaces;
    using Abp.Domain.Entities;
    using System.Collections.Generic;

    public class Capability : FullAuditedEntity<long>, IIndexed, IDescribed, IPassivable, INamed
    {
        public Capability(long evaluationId, byte index, string description, bool isActive, string name)
        {
            EvaluationId = evaluationId;
            Index = index;
            Description = description;
            IsActive = isActive;
            Name = name;
        }

        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public  virtual Evaluation Evaluation { get; protected set; }
        public virtual byte Index { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }
        [ForeignKey("CapabilityId")]
        public virtual ICollection<UserCapability> UserCapabilities { get; protected set; }
        public string Name { get; set; }
    }
}