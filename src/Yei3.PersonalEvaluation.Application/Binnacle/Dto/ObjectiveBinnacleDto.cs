using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;

namespace Yei3.PersonalEvaluation.Binnacle.Dto
{
    [AutoMap(typeof(ObjectiveBinnacleValueObject), typeof(ObjectiveBinnacle))]
    public class ObjectiveBinnacleDto : ObjectiveBinnacleValueObject, IEntityDto<long>
    {
        
    }
}