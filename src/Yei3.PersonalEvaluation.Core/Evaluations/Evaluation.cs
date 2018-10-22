namespace Yei3.PersonalEvaluation.Evaluations
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities.Auditing;
    using Authorization.Users;
    using Term;
    using System.Collections.Generic;
    using Capabilities;
    using Objectives;
    using Identity;
    using Interfaces;
    using JetBrains.Annotations;

    public class Evaluation : FullAuditedEntity<long>, ICommented, ISigned
    {
        public Evaluation(EvaluationTerm term, long evaluatedUserId, long evaluatorUserId)
        {
            Term = term;
            EvaluatedUserId = evaluatedUserId;
            EvaluatorUserId = evaluatorUserId;
        }

        public virtual EvaluationTerm Term { get; protected set; }
        public virtual long EvaluatedUserId { get; protected set; }
        [ForeignKey("EvaluatedUserId")]
        public virtual User EvaluatedUser { get; protected set; }
        public virtual long EvaluatorUserId { get; protected set; }
        [ForeignKey("EvaluatorUserId")]
        public virtual User EvaluatorUser { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual ICollection<Capability> Capabilities { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual ICollection<Objective> Objectives { get; protected set; }
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

    }
}
