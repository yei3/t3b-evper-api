namespace Yei3.PersonalEvaluation.Evaluations.Objectives
{
    using Abp.Domain.Entities.Auditing;
    using Authorization.Users;
    using System.ComponentModel.DataAnnotations.Schema;
    using Interfaces;

    public class UserObjective : FullAuditedEntity<long>, IAccomplished
    {
        public virtual long UserId { get; protected set; }
        [ForeignKey("UserId")]
        public virtual User User { get; protected set; }
        public virtual long ObjectiveId { get; protected set; }
        [ForeignKey("ObjectiveId")]
        public virtual Objective Objective { get; protected set; }

        public virtual bool IsAccomplished { get; set; }
    }
}
