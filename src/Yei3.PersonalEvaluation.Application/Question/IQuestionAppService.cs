using Abp.Application.Services;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.Question
{
    public interface IQuestionAppService : IAsyncCrudAppService<QuestionDto, long, QuestionGetAllInputDto, QuestionDto>
    {
        
    }
}