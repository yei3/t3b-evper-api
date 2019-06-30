using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Yei3.PersonalEvaluation.Report.Dto;

namespace Yei3.PersonalEvaluation.Report
{
    public interface IEvaluationReportAppService : IApplicationService
    {
        Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesReport(long? period = null);
        Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesAccomplishmentReport(long? period = null);
        Task<IList<SalesCapabilitiesReportDto>> GetCollaboratorAccomplishmentReport(long? period = null);
        Task<AdministratorObjectiveReportDto> GetAdministratorObjectivesReport(AdministratorInputDto input);
        Task<IList<CapabilitiesReportDto>> GetAdministratorCapabilitiesReport(AdministratorInputDto input);
        Task<ICollection<EvaluationResultsDto>> GetEvaluationCollaboratorResultsById(UserEvaluationResultDto input);
        Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesReportById(UserEvaluationResultDto input);
    }
}