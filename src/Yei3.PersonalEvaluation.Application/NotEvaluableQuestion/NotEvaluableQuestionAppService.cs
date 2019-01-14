using System.Threading.Tasks;
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

        public override async Task<NotEvaluableQuestionDto> Create(NotEvaluableQuestionDto input)
        {
            NotEvaluableQuestionDto notEvaluableQuestion = await base.Create(input);
            await CurrentUnitOfWork.SaveChangesAsync();

            Evaluations.EvaluationQuestions.NotEvaluableQuestion currentQuestion = await Repository.SingleAsync(question => question.Id == notEvaluableQuestion.Id);

            currentQuestion.SetAnswer(notEvaluableQuestion.Id);

            return notEvaluableQuestion;
        }
    }
}