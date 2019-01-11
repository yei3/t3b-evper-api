using Abp.Application.Services;
using Abp.Domain.Repositories;
using Yei3.PersonalEvaluation.EvaluationObjectives.Dto;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.NotEvaluableAnswers.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableAnswers
{
    public class NotEvaluableAnswerAppService : AsyncCrudAppService<NotEvaluableAnswer, NotEvaluableAnswerDto, long, GetAllEvaluationObjectivesDto>, INotEvaluableAnswerAppService
    {
        public NotEvaluableAnswerAppService(IRepository<NotEvaluableAnswer, long> repository) : base(repository)
        {
        }
    }
}