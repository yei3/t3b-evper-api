namespace Yei3.PersonalEvaluation.Evaluations.Capabilities
{
    using Abp.Domain.Entities.Auditing;
    using Interfaces;
    using System.ComponentModel.DataAnnotations.Schema;
    using Authorization.Users;

    public class UserCapability : FullAuditedEntity<long>, IRated, ICommented
    {
        public virtual long UserId { get; protected set; }
        [ForeignKey("UserId")]
        public virtual User User { get; protected set; }
        public virtual long CapabilityId { get; protected set; }
        [ForeignKey("CapabilityId")]
        public virtual Capability Capability { get; protected set; }
        public virtual float Rate { get; set; }
        public virtual string Comment { get; set; }
    }
}