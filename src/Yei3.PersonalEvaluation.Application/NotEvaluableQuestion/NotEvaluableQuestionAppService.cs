using Abp.Application.Services;
using Abp.Domain.Repositories;
using Yei3.PersonalEvaluation.NotEvaluableQuestion.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableQuestion
{
    public class NotEvaluableQuestionAppService : AsyncCrudAppService<Evaluations.EvaluationQuestions.NotEvaluableQuestion, NotEvaluableQuestionDto, long, QuestionGetAllInputDto>, INotEvaluableQuestionAppService
    {
        public NotEvaluableQuestionAppService(IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> repository) : base(repository)
        {
        }
    }
}