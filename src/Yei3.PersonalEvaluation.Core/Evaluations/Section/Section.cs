namespace Yei3.PersonalEvaluation.Evaluations.Section
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using Interfaces;
    public class Section : FullAuditedEntity<long>, INamed, IPassivable
    {
        public Section(string name, bool showName, long evaluationId, long? parentId, bool isActive)
        {
            Name = name;
            ShowName = showName;
            EvaluationId = evaluationId;
            ParentId = parentId;
            IsActive = isActive;
        }

        public string Name { get; protected set; }
        public bool ShowName { get; protected set; }
        public long EvaluationId { get; protected set; }

        [ForeignKey("EvaluationId")]
        public Evaluation Evaluation { get; protected set; }
        public long? ParentId { get; protected set; }
        [ForeignKey("ParentId")]
        public Section ParentSection { get; protected set; }

        public bool IsActive { get; set; }
        public ICollection<Question.Question> Questions { get; protected set; }
    }
}