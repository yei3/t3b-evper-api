using Abp.Application.Services;
using Abp.Domain.Repositories;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.Question
{
    public class QuestionAppService : AsyncCrudAppService<Evaluations.Questions.UnmeasuredQuestion, QuestionDto, long, QuestionGetAllInputDto, QuestionDto>, IQuestionAppService
    {
        public QuestionAppService(IRepository<Evaluations.Questions.UnmeasuredQuestion, long> repository) : base(repository)
        {
        }
    }
}