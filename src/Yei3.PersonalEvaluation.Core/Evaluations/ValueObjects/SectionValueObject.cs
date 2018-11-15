
namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;
    using System.Collections.Generic;

    public class SectionValueObject : ValueObject<SectionValueObject>
    {
        public string Name { get; set; }
        public bool ShowName { get; set; }
        public long EvaluationId { get; set; }
        public long? ParentId { get; set; }

        public ICollection<SectionValueObject> SubSections { get; set; }
        public ICollection<QuestionValueObject> Questions { get; set; }
    }
}