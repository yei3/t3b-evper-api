using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yei3.PersonalEvaluation.Evaluations
{
    using Dto;

    public interface IEvaluationAppService
    {
        Task ApplyEvaluationTemplate(CreateEvaluationDto input);        
        Task Delete(long id);
        Task<EvaluationDto> Get(long id);
        Task<ICollection<EvaluationDto>> GetAll();
        Task ClosingComment(EvaluationCloseDto inpunt);
        Task<ICollection<AdministratorEvaluationSummaryDto>> GetAdministratorEvaluationSummary();
    }
}