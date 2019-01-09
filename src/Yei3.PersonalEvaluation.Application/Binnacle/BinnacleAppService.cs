using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Yei3.PersonalEvaluation.Binnacle.Dto;

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
                .Where(binnacleEntry => binnacleEntry.EvaluationMeasuredQuestionId == input.EvaluationMeasuredQuestionId);
        }
    }
}