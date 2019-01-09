using Abp.Application.Services;
using Yei3.PersonalEvaluation.Binnacle.Dto;

namespace Yei3.PersonalEvaluation.Binnacle
{
    public interface IBinnacleAppService : IAsyncCrudAppService<ObjectiveBinnacleDto, long, ObjectiveBinnacleGetAllInputDto>
    {
        
    }
}