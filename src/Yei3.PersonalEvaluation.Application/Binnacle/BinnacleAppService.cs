using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Binnacle.Dto;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Binnacle
{
    public class BinnacleAppService : AsyncCrudAppService<ObjectiveBinnacle, ObjectiveBinnacleDto, long, ObjectiveBinnacleGetAllInputDto>, IBinnacleAppService
    {
        public BinnacleAppService(IRepository<ObjectiveBinnacle, long> repository) : base(repository)
        {
        }

        protected override IQueryable<ObjectiveBinnacle> CreateFilteredQuery(ObjectiveBinnacleGetAllInputDto input)
        {
            return base.CreateFilteredQuery(input)
                .Where(binnacleEntry => binnacleEntry.EvaluationQuestionId == input.EvaluationMeasuredQuestionId);
        }

        public override async Task<ObjectiveBinnacleDto> Create(ObjectiveBinnacleDto input)
        {
            ObjectiveBinnacleDto binnacleDto = await base.Create(input);
            await CurrentUnitOfWork.SaveChangesAsync();
            ObjectiveBinnacle binnacle = await Repository
                .GetAll()
                .Include(objectiveBinnacle => objectiveBinnacle.EvaluationQuestion)
                .SingleAsync(objectiveBinnacle => objectiveBinnacle.Id == binnacleDto.Id);

            binnacle.EvaluationQuestion.Status = binnacle.EvaluationQuestion.Status < EvaluationQuestionStatus.Initiated
                ? EvaluationQuestionStatus.Initiated
                : binnacle.EvaluationQuestion.Status;

            return binnacleDto;
        }
    }
}