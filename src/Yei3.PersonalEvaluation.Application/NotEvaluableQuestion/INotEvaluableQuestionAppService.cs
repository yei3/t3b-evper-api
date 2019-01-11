using Abp.Application.Services;
using Yei3.PersonalEvaluation.NotEvaluableQuestion.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableQuestion
{
    public interface INotEvaluableQuestionAppService : IAsyncCrudAppService<NotEvaluableQuestionDto, long, QuestionGetAllInputDto>
    {

    }
}