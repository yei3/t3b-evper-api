using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Yei3.PersonalEvaluation.Report.Dto;

namespace Yei3.PersonalEvaluation.Report
{
    public interface IEvaluationReportAppService : IApplicationService
    {
        Task<ICollection<EvaluationResultsDto>> GetEvaluationResults();
        Task<ICollection<EvaluationResultsDto>> GetEvaluationCollaboratorResults();
        Task<ICollection<EvaluationResultsDto>> GetEvaluationSupervisorResults();
        Task<EvaluationResultDetailsDto> GetEvaluationResultDetail(long evaluationTemplateId);
        Task<EvaluationsComparisonDto> GetEvaluationComparision(EvaluationsComparisonInputDto input);
    }
}