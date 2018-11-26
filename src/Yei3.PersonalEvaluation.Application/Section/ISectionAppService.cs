using Abp.Application.Services;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;
using Yei3.PersonalEvaluation.Section.Dto;

namespace Yei3.PersonalEvaluation.Section
{
    public interface ISectionAppService : IAsyncCrudAppService<SectionDto, long, SectionGetAllInputDto, SectionCreateInputDto>
    {
        
    }
}