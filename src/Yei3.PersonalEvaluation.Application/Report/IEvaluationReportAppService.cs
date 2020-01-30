using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Yei3.PersonalEvaluation.Application.Report.Dto;
using Yei3.PersonalEvaluation.Report.Dto;

namespace Yei3.PersonalEvaluation.Report
{
    public interface IEvaluationReportAppService : IApplicationService
    {
        Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesReport(long? period = null);
        Task<IList<CapabilitiesReportDto>> GetCollaboratorCompetencesReport(long? period = null);
        Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesSalesReport(long? period = null);
        Task<IList<SalesCapabilitiesReportDto>> GetCollaboratorCapabilitiesSalesReport(long? period = null);
        Task<AdministratorObjectiveReportDto> GetEvaluatorObjectivesReport(AdministratorInputDto input);
        Task<IList<CapabilitiesReportDto>> GetEvaluatorCapabilitiesReport(AdministratorInputDto input);
        Task<AdministratorObjectiveReportDto> GetEvaluatorObjectivesSalesReport(AdministratorInputDto input);
        Task<IList<SalesCapabilitiesReportDto>> GetEvaluatorCapabilitiesSalesReport(AdministratorInputDto input);
        Task<AdministratorObjectiveReportDto> GetAdministratorObjectivesReport(AdministratorInputDto input);
        Task<IList<CapabilitiesReportDto>> GetAdministratorCapabilitiesReport(AdministratorInputDto input);
        Task<AdministratorObjectiveReportDto> GetAdministratorObjectivesSalesReport(AdministratorInputDto input);
        Task<IList<SalesCapabilitiesReportDto>> GetAdministratorCapabilitiesSalesReport(AdministratorInputDto input);
        Task<EvaluationEmployeeDataDto> GetEvaluationEmployeeData(AdministratorInputDto input);
        Task<EvaluationEmployeeDataDto> GetAdministratorEvaluationEmployeeData(AdministratorInputDto input);
    }
}