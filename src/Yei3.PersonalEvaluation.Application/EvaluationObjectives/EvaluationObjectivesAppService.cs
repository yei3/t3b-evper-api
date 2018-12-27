using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.EvaluationObjectives.Dto;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;

namespace Yei3.PersonalEvaluation.EvaluationObjectives
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

        protected override async Task<MeasuredAnswer> GetEntityByIdAsync(long id)
        {
            return await Repository
                .GetAll()
                .Include(answer => answer.EvaluationMeasuredQuestion)
                .SingleOrDefaultAsync(answer => answer.Id == id);
        }

        public override async Task<EvaluationObjectiveDto> Update(EvaluationObjectiveDto input)
        {
            Evaluation evaluation = Repository
                .GetAll()
                .Include(answer => answer.EvaluationMeasuredQuestion)
                .ThenInclude(question => question.Evaluation)
                .Single(answer => answer.Id == input.Id)
                .EvaluationMeasuredQuestion
                .Evaluation;

            if (evaluation.Status == EvaluationStatus.NonInitiated)
            {
                evaluation.UnfinishEvaluation();
            }

            return await base.Update(input);
        }
    }
}