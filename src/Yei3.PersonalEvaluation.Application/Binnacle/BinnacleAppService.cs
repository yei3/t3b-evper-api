using Abp.Application.Services;
using Abp.Domain.Repositories;
using Yei3.PersonalEvaluation.Binnacle.Dto;

namespace Yei3.PersonalEvaluation.Binnacle
{
    public class BinnacleAppService : AsyncCrudAppService<ObjectiveBinnacle, ObjectiveBinnacleDto, long>, IBinnacleAppService
    {
        public BinnacleAppService(IRepository<ObjectiveBinnacle, long> repository) : base(repository)
        {
        }
    }
}