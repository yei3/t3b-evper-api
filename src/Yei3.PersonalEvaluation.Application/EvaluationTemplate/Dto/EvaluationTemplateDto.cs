using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Yei3.PersonalEvaluation.EvaluationTemplate.Dto
{
    [AutoMap(typeof(Evaluations.EvaluationTemplates.EvaluationTemplate))]
    public class EvaluationTemplateDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public ICollection<SectionDto> Sections { get; set; }
        public bool IsAutoEvaluation { get; set; }
        public bool IncludePastObjectives { get; set; }

        public void PurgeSubSections()
        {
            Sections = Sections.Where(section => !section.ParentId.HasValue).ToList();
        }
    }
}