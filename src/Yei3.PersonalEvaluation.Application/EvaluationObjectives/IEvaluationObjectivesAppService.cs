using Abp.Application.Services;
using Yei3.PersonalEvaluation.EvaluationObjectives.Dto;

namespace Yei3.PersonalEvaluation.EvaluationObjectives
{
    public interface IEvaluationObjectivesAppService : IAsyncCrudAppService<EvaluationObjectiveDto, long, GetAllEvaluationObjectivesDto>
    {
        
    }
}