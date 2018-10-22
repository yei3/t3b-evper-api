namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Application.Services;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Dto;


    public interface IEvaluationAppService : IApplicationService
    {
        Task<EntityDto<long>> CreateEvaluationAndGetIdAsync(CreateEvaluationDto createEvaluationDto);
    }
}