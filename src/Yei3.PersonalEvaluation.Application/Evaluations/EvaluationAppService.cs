namespace Yei3.PersonalEvaluation.Evaluations
{
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Dto;
    using Abp.Authorization;
    using Authorization;

    public class EvaluationAppService : PersonalEvaluationAppServiceBase, IEvaluationAppService
    {
        private readonly IEvaluationManager EvaluationManager;

        public EvaluationAppService(IEvaluationManager evaluationManager)
        {
            EvaluationManager = evaluationManager;
        }

        [AbpAuthorize(PermissionNames.AdministrationEvaluationCreate)]
        public async Task<EntityDto<long>> CreateEvaluationAndGetIdAsync(CreateEvaluationDto createEvaluationDto)
        {
            return new EntityDto<long>(await EvaluationManager.CreateEvaluationAndGetIdAsync(createEvaluationDto));
        }
    }
}