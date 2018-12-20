using System.Linq;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.EvaluationAnswer.Dto;
using Yei3.PersonalEvaluation.EvaluationObjectives.Dto;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;

namespace Yei3.PersonalEvaluation.EvaluationAnswer
{
    public class UnmeasuredAnswerAppService : AsyncCrudAppService<UnmeasuredAnswer, UnmeasuredAnswerDto, long, GetAllEvaluationObjectivesDto>, IUnmeasuredQuestionAppService
    {
        public UnmeasuredAnswerAppService(IRepository<UnmeasuredAnswer, long> repository) : base(repository)
        {
        }

        protected override IQueryable<UnmeasuredAnswer> CreateFilteredQuery(GetAllEvaluationObjectivesDto input)
        {
            IQueryable<UnmeasuredAnswer> evaluationMeasuredAnswers = Repository
                .GetAll()
                .Include(unmeasuredAnswer => unmeasuredAnswer.EvaluationUnmeasuredQuestion)
                .ThenInclude(evaluation => evaluation.UnmeasuredQuestion)
                .Include(unmeasuredAnswer => unmeasuredAnswer.EvaluationUnmeasuredQuestion.UnmeasuredQuestion)
                .ThenInclude(question => question.Section)
                .Where(unmeasuredAnswer => unmeasuredAnswer.EvaluationUnmeasuredQuestion.EvaluationId == input.EvaluationId);

            return evaluationMeasuredAnswers;
        }
    }
}