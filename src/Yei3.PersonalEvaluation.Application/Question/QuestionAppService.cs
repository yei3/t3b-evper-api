using Abp.Application.Services;
using Abp.Domain.Repositories;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.Question
{
    public class QuestionAppService : AsyncCrudAppService<Evaluations.Questions.Question, QuestionDto, long, QuestionGetAllInputDto, QuestionDto>, IQuestionAppService
    {
        public QuestionAppService(IRepository<Evaluations.Questions.Question, long> repository) : base(repository)
        {
        }
    }
}