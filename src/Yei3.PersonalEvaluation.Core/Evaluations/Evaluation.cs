namespace Yei3.PersonalEvaluation.Evaluations
{
    using Interfaces;
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities.Auditing;
    using Authorization.Users;
    using Term;
    using System.Collections.Generic;
    using Capabilities;
    using Objectives;

    public class Evaluation : FullAuditedEntity<long>, INamed, IDescribed
    {
        public Evaluation(EvaluationTerm term, long evaluatorUserId)
        {
            Term = term;
            EvaluatorUserId = evaluatorUserId;
        }

        public virtual EvaluationTerm Term { get; protected set; }
        public virtual long EvaluatorUserId { get; protected set; }
        [ForeignKey("EvaluatorUserId")]
        public virtual User EvaluatorUser { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual ICollection<Capability> Capabilities { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual ICollection<Objective> Objectives { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual ICollection<EvaluationUser> EvaluationUsers { get; protected set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
    }
}
