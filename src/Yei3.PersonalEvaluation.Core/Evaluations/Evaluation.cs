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
        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual EvaluationTemplate Template { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual EvaluationRevision Revision { get; protected set; }
        public virtual long UserId { get; protected set; }
        [ForeignKey("UserId")]
        public virtual User User { get; protected set; }
        public virtual EvaluationStatus Status { get; protected set; }
        public virtual EvaluationTerm Term { get; protected set; }
        public virtual DateTime EndDateTime { get; protected set; }
        public virtual ICollection<EvaluationQuestion> Questions { get; protected set; }
    }
}
