using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;
using Yei3.PersonalEvaluation.Evaluations.EvaluationTemplates;
using Yei3.PersonalEvaluation.Evaluations.Terms;

namespace Yei3.PersonalEvaluation.Evaluations
{
    public class Evaluation : FullAuditedEntity<long>
    {
        public Evaluation(string name, long evaluationId, long userId, DateTime startDateTime, DateTime endDateTime)
        {
            Name = name;
            EvaluationId = evaluationId;
            UserId = userId;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Term = startDateTime.Month < 6 ? EvaluationTerm.FirstSemester : EvaluationTerm.SecondSemester;
            Status = EvaluationStatus.NonInitiated;
            Questions = new List<EvaluationQuestion>();
        }

        public virtual string Name { get; protected set; }
        public virtual string ClosingComment { get; set; }
        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual EvaluationTemplate Template { get; protected set; }
        public virtual long RevisionId { get; protected set; }

        [ForeignKey("RevisionId")]
        public virtual EvaluationRevision Revision { get; protected set; }
        public virtual long UserId { get; protected set; }
        [ForeignKey("UserId")]
        public virtual User User { get; protected set; }
        public virtual EvaluationStatus Status { get; protected set; }
        public virtual EvaluationTerm Term { get; protected set; }
        public virtual DateTime StartDateTime { get; protected set; }
        public virtual DateTime EndDateTime { get; protected set; }
        public virtual ICollection<EvaluationQuestion> Questions { get; protected set; }

        public void SetRevision(long evaluationId, long reviewerUserId, DateTime revisionDateTime)
        {
            Revision = new EvaluationRevision(evaluationId, reviewerUserId, revisionDateTime);
        }

        public void FinishEvaluation()
        {
            Status = EvaluationStatus.Finished;
        }

        public void UnfinishEvaluation()
        {
            Status = EvaluationStatus.Pending;
        }

        public void ValidateEvaluation()
        {
            Status = EvaluationStatus.Validated;
        }

        public void InvalidateEvaluation()
        {
            Status = EvaluationStatus.Finished;
        }

        public void ScheduleReview()
        {
            Status = EvaluationStatus.PendingReview;
        }
    }
}
