using Yei3.PersonalEvaluation.Evaluations.Capabilities;

namespace Yei3.PersonalEvaluation.Evaluations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities.Auditing;
    using JetBrains.Annotations;
    using Authorization.Users;
    using Objectives;
    using Identity;
    using Interfaces;

    public class EvaluationUser : FullAuditedEntity<long>, ICommented, ISigned
    {
        public EvaluationUser(long userId, long evaluationId)
        {
            UserId = userId;
            EvaluationId = evaluationId;
        }

        public virtual long UserId { get; protected set; }
        [ForeignKey("UserId")]
        public virtual User User { get; protected set; }
        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual Evaluation Evaluation { get; protected set; }
        public virtual long? UserSignatureId { get; set; }
        [CanBeNull]
        [ForeignKey("UserSignatureId")]
        public virtual UserSignature UserSignature { get; protected set; }
        [CanBeNull]
        public virtual string Comment { get; set; }
        [CanBeNull]
        public virtual string Strengths { get; protected set; }
        [CanBeNull]
        public virtual string ImprovementsArea { get; protected set; }
        [CanBeNull]
        public virtual string DevelopmentPlan { get; protected set; }
        [ForeignKey("NextEvaluationId")]
        public virtual ICollection<Objective> NextTermObjectives { get; protected set; }

        [ForeignKey("UserId")]
        public virtual ICollection<EvaluationUserObjective> EvaluationUserObjectives { get; protected set; }
        [ForeignKey("UserId")]
        public virtual ICollection<EvaluationUserCapability> EvaluationUserCapabilities { get; protected set; }
    }
}