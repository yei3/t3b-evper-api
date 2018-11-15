namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using System.Collections.Generic;
    using Abp.AutoMapper;
    using ValueObjects;

    [AutoMap(typeof(SectionValueObject), typeof(Section.Section))]
    public class SectionDto
    {
        public string Name { get; set; }
        public bool ShowName { get; set; }
        public long EvaluationId { get; set; }
        public long? ParentId { get; set; }

        public ICollection<SectionDto> SubSections { get; set; }
        public ICollection<QuestionDto> Questions { get; set; }
    }
}