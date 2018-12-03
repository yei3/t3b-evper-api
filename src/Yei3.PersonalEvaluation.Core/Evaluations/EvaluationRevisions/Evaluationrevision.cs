using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions
{
    public class EvaluationRevision : FullAuditedEntity<long>
    {
        public EvaluationRevision(long evaluationId, long reviewerUserId, DateTime revisionDateTime)
        {
            EvaluationId = evaluationId;
            ReviewerUserId = reviewerUserId;
            RevisionDateTime = revisionDateTime;
            Status = EvaluationRevisionStatus.Pending;
        }

        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual Evaluation Evaluation { get; protected set; }
        public virtual long ReviewerUserId { get; protected set; }
        [ForeignKey("ReviewerUserId")]
        public virtual User ReviewerUser { get; protected set; }
        public virtual EvaluationRevisionStatus Status { get; protected set; }
        public virtual DateTime RevisionDateTime { get; protected set; }
    }
}