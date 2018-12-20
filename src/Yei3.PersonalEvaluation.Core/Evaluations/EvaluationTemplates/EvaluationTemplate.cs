using System.Collections.Generic;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Evaluations.Sections;
using Yei3.PersonalEvaluation.Interfaces;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationTemplates
{
    public class EvaluationTemplate : FullAuditedEntity<long>, INamed, IDescribed
    {
        public virtual ICollection<Section> Sections { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual string Description { get; protected set; }
        public virtual string Instructions { get; protected set; }
        public virtual bool IsAutoEvaluation { get; protected set; }
        public virtual ICollection<Evaluation> Evaluations { get; protected set; }
    }
}