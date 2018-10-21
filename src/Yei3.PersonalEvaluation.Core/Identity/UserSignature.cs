namespace Yei3.PersonalEvaluation.Identity
{
    using Abp.Domain.Entities.Auditing;
    using Authorization.Users;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserSignature : FullAuditedEntity<long>
    {
        public virtual long UserId { get; protected set; }
        [ForeignKey("UserId")]
        public virtual User User { get; protected set; }
    }
}