using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Yei3.PersonalEvaluation.Report.Dto;

namespace Yei3.PersonalEvaluation.Report
{
    public interface IEvaluationReportAppService : IApplicationService
    {
        Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesReport(long? userId = null);
        Task<List<CollaboratorCompetencesReportDto>> GetCollaboratorCompetencesReport();
        Task<ICollection<EvaluationResultsDto>> GetEvaluationResults();
        Task<ICollection<EvaluationResultsDto>> GetEvaluationCollaboratorResults();
        Task<ICollection<EvaluationResultsDto>> GetEvaluationSupervisorResults();
        Task<EvaluationResultDetailsDto> GetEvaluationResultDetail(long evaluationTemplateId);
        Task<EvaluationsComparisonDto> GetEvaluationComparision(EvaluationsComparisonInputDto input);
        Task<EvaluationsComparisonDto> GetCollaboratorEvaluationComparision(UserEvaluationsComparisonInputDto input);
        Task<ICollection<EvaluationsCollaboratorComparisonDto>> GetSupervisorEvaluationComparision(UserEvaluationsComparisonInputDto input);
        Task<AdministratorObjectiveReportDto> GetAdministratorObjectivesReport(AdministratorObjectiveReportInputDto input);
        Task<CollaboratorCompetencesReportDto> GetAdministratorCapabilitiesReport(AdministratorObjectiveReportInputDto input);
    }
}