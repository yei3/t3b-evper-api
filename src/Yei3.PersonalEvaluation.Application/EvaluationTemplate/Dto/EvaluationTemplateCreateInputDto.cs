using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Yei3.PersonalEvaluation.EvaluationTemplate.Dto
{
    [AutoMap(typeof(Evaluations.EvaluationTemplates.EvaluationTemplate))]
    public class EvaluationTemplateCreateInputDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
    }
}