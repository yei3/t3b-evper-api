namespace Yei3.PersonalEvaluation.Evaluations
{
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Dto;
    using Abp.Authorization;
    using Authorization;
    using Abp.Domain.Entities;
    using Abp.UI;

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

        [AbpAuthorize(PermissionNames.AdministrationEvaluationObjectivesManage)]
        public async Task<EntityDto<long>> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveDto addEvaluationObjectiveDto)
        {
            try
            {
                return new EntityDto<long>(await EvaluationManager.AddEvaluationObjectiveAndGetIdAsync(addEvaluationObjectiveDto));
            }
            catch (EntityNotFoundException e)
            {
                throw new UserFriendlyException(L(e.Message));
            }
        }

        [AbpAuthorize(PermissionNames.AdministrationEvaluationCapabilitiesManage)]
        public async Task<EntityDto<long>> AddEvaluationCapabilityAndGetIdAsync(AddEvaluationCapabilityDto addEvaluationCapabilityDto)
        {
            try
            {
                return new EntityDto<long>(await EvaluationManager.AddEvaluationCapabilityAndGetIdAsync(addEvaluationCapabilityDto));
            }
            catch (EntityNotFoundException e)
            {
                throw new UserFriendlyException(L(e.Message));
            }
        }
    }
}