using System.Linq;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;

namespace Yei3.PersonalEvaluation.EvaluationObjectives.Dto
{
    public class EvaluationObjectivesAppService : AsyncCrudAppService<MeasuredAnswer, EvaluationObjectiveDto, long, GetAllEvaluationObjectivesDto>, IEvaluationObjectivesAppService
    {
        public EvaluationObjectivesAppService(IRepository<MeasuredAnswer, long> repository) : base(repository)
        {
        }

        protected override IQueryable<MeasuredAnswer> CreateFilteredQuery(GetAllEvaluationObjectivesDto input)
        {
            IQueryable<MeasuredAnswer> evaluationMeasuredAnswers = Repository
                .GetAll()
                .Include(measuredAnswer => measuredAnswer.EvaluationMeasuredQuestion)
                .ThenInclude(evaluation => evaluation.MeasuredQuestion)
                .Include(measuredAnswer => measuredAnswer.EvaluationMeasuredQuestion.MeasuredQuestion)
                .ThenInclude(question => question.Section)
                .Where(measuredAnswer => measuredAnswer.EvaluationMeasuredQuestion.EvaluationId == input.EvaluationId);

            return evaluationMeasuredAnswers;
        }
    }
}