using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Application.Services.Dto;
    using Dto;

    public interface IEvaluationAppService
    {
        Task ApplyEvaluationTemplate(CreateEvaluationDto input);        
        Task Delete(long id);
        Task<EvaluationDto> Get(long id);
        Task<ICollection<EvaluationDto>> GetAll();
        Task ClosingComment(EvaluationCloseDto inpunt);
        Task FinalizeEvaluation(EntityDto<long> input);
        Task ReopenEvaluation(EntityDto<long> input);
        Task<ICollection<AdministratorEvaluationSummaryDto>> GetAdministratorEvaluationSummary();
        Task<PagedResultDto<EvaluationStatusListItemDto>> GetEvaluationsStatus(EvaluationStatusInputDto input);
        FileDto GetEvaluationsStatusSheet(EvaluationStatusInputDto input);
        Task<PagedResultDto<EvaluationResultListItemDto>> GetEvaluationsResult(EvaluationResultInputDto input);
        FileDto GetEvaluationsResultSheet(EvaluationResultInputDto input);
    }
}