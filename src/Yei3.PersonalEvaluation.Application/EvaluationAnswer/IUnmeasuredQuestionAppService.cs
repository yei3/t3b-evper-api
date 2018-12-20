using Abp.Application.Services;
using Yei3.PersonalEvaluation.EvaluationAnswer.Dto;
using Yei3.PersonalEvaluation.EvaluationObjectives.Dto;

namespace Yei3.PersonalEvaluation.EvaluationAnswer
{
    public interface IUnmeasuredQuestionAppService : IAsyncCrudAppService<UnmeasuredAnswerDto, long, GetAllEvaluationObjectivesDto>
    {
        
    }
}