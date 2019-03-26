using Abp.Application.Services;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;

namespace Yei3.PersonalEvaluation.EvaluationTemplate
{
    public interface IEvaluationTemplateAppService : IAsyncCrudAppService<EvaluationTemplateDto, long, GetAllEvaluationTemplateInputDto, EvaluationTemplateCreateInputDto, EvaluationTemplateCreateInputDto>
    {
        
    }
}