using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Yei3.PersonalEvaluation.Evaluations.EvaluationTemplates;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.Interfaces;

namespace Yei3.PersonalEvaluation.Evaluations.Sections
{
    public class Section : FullAuditedEntity<long>, INamed, IPassivable
    {
        public virtual string Name { get; protected set; }
        public virtual bool DisplayName { get; protected set; }
        public virtual long EvaluationTemplateId { get; protected set; }

        [ForeignKey("EvaluationTemplateId")]
        public virtual EvaluationTemplate Template { get; protected set; }
        public virtual long? ParentId { get; protected set; }
        [ForeignKey("ParentId")]
        [InverseProperty(nameof(ChildSections))]
        public virtual Section ParentSection { get; protected set; }
        public virtual ICollection<Section> ChildSections { get; protected set; }

        public virtual bool IsActive { get; set; }
        public virtual ICollection<Question> Questions { get; protected set; }
    }
}