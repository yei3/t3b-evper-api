using Abp.Application.Services;
using Yei3.PersonalEvaluation.EvaluationObjectives.Dto;
using Yei3.PersonalEvaluation.NotEvaluableAnswers.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableAnswers
{
    public interface INotEvaluableAnswerAppService : IAsyncCrudAppService<NotEvaluableAnswerDto, long, GetAllEvaluationObjectivesDto>
    {
        
    }
}