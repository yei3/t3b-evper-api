namespace Yei3.PersonalEvaluation.Identity
{
    using System.Collections.Generic;
    using Evaluations;
    using Abp.Domain.Entities.Auditing;
    using Authorization.Users;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserSignature : FullAuditedEntity<long>
    {
        public virtual long UserId { get; protected set; }
        [ForeignKey("UserId")]
        public virtual User User { get; protected set; }
        [ForeignKey("UserSignatureId")]
        public virtual ICollection<EvaluationUser> EvaluationUsers { get; protected set; }
    }
}