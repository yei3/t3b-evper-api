namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Application.Services;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Dto;

    public interface IEvaluationAppService : IApplicationService
    {
        Task<EntityDto<long>> CreateEvaluationAndGetIdAsync(CreateEvaluationDto createEvaluationDto);
        Task<EntityDto<long>> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveDto addEvaluationObjectiveDto);
        Task<EntityDto<long>> AddEvaluationCapabilityAndGetIdAsync(AddEvaluationCapabilityDto addEvaluationCapabilityDto);
    }
}