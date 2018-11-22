using System.Collections.Generic;
using JetBrains.Annotations;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.AutoMapper;
    using ValueObjects;

    [AutoMap(typeof(SectionValueObject), typeof(SubsectionValueObject), typeof(Section.Section))]
    public class SectionDto
    {
        public long? Id { get; set; }
        [CanBeNull] public string Name { get; set; }
        public long? EvaluationId { get; set; }
        public long? ParentId { get; set; }
        [CanBeNull] public ICollection<QuestionDto> Questions { get; set; }
    }
}