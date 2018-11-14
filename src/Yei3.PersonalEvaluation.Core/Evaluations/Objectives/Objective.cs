using System;

namespace Yei3.PersonalEvaluation.Evaluations.Objectives
{
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using Abp.Domain.Entities;
    using Interfaces;

    public class Objective : FullAuditedEntity<long>, IIndexed, IDescribed, IPassivable
    {
        public Objective(byte index, string description, long evaluationId, bool isActive, string definitionOfDone, DateTime deliveryDate)
        {
            Index = index;
            Description = description;
            EvaluationId = evaluationId;
            IsActive = isActive;
            DefinitionOfDone = definitionOfDone;
            DeliveryDate = deliveryDate;
        }

        public virtual byte Index { get; set; }
        public virtual string Description { get; set; }
        public virtual string DefinitionOfDone { get; protected set; }
        public virtual DateTime DeliveryDate { get; protected set; }
        public virtual long EvaluationId { get; protected set; }
        [ForeignKey("EvaluationId")]
        public virtual Evaluation Evaluation { get; protected set; }
        public virtual bool IsActive { get; set; }
        [ForeignKey("ObjectiveId")]
        public  virtual ICollection<EvaluationUserObjective> EvaluationUserObjectives { get; protected set; }
        public virtual long? NextEvaluationId { get; protected set; }
        [ForeignKey("NextEvaluationId")]
        public virtual Evaluation NextEvaluation { get; protected set; }
    }
}
