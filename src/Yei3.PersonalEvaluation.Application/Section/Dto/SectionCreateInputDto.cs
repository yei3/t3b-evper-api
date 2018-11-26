using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Yei3.PersonalEvaluation.Section.Dto
{
    [AutoMap(typeof(Evaluations.Sections.Section))]
    public class SectionCreateInputDto : EntityDto<long>
    {
        public string Name { get; set; }
        public bool DisplayName { get; set; }
        public long EvaluationTemplateId { get; set; }
        public long? ParentId { get; set; }

    }
}