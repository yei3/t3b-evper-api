using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JetBrains.Annotations;
using Yei3.PersonalEvaluation.Objective.Dto;

namespace Yei3.PersonalEvaluation.EvaluationTemplate.Dto
{
    [AutoMap(typeof(Evaluations.Sections.Section))]
    public class SectionDto : EntityDto<long>
    {
        [CanBeNull] public string Name { get; set; }
        public bool DisplayName { get; set; }
        public long? EvaluationTemplateId { get; set; }
        public long? ParentId { get; set; }
        [CanBeNull] public ICollection<QuestionDto> UnmeasuredQuestions { get; set; }
        [CanBeNull] public ICollection<ObjectiveDto> MeasuredQuestions { get; set; }
        [CanBeNull] public ICollection<SectionDto> ChildSections { get; set; }
        [CanBeNull] public float Value { get; set; }
    }
}