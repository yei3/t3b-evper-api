using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Report.Dto;

namespace Yei3.PersonalEvaluation.Report
{
    //[AbpAuthorize(PermissionNames.AdministrationEvaluationReports)]
    public class EvaluationReportAppService : ApplicationService, IEvaluationReportAppService
    {
        private readonly IRepository<Evaluation, long> EvaluationRepository;

        public EvaluationReportAppService(IRepository<Evaluation, long> evaluationRepository)
        {
            EvaluationRepository = evaluationRepository;
        }

        public async Task<ICollection<EvaluationResultsDto>> GetEvaluationResults()
        {
            var groupedEvaluations = EvaluationRepository
                .GetAll()
                .GroupBy(evaluation => new
                {
                    Id = evaluation.EvaluationId,
                    CreationTime = evaluation.CreationTime.DayOfYear,
                    StartTime = evaluation.StartDateTime,
                    EndTime = evaluation.EndDateTime,
                    CreatorUserId = evaluation.CreatorUserId,
                    Term = evaluation.Term
                });

            var evaluations = new List<EvaluationResultsDto>();

            foreach (var groupedEvaluation in groupedEvaluations)
            {
                Evaluation firstEvaluation = groupedEvaluation.First();
                evaluations.Add(new EvaluationResultsDto()
                {
                    Id = groupedEvaluation.Key.Id,
                    Status = firstEvaluation.StartDateTime < DateTime.Now
                        ? EvaluationStatus.NonInitiated
                        : firstEvaluation.EndDateTime <= DateTime.Now
                            ? EvaluationStatus.Finished
                            : EvaluationStatus.NonInitiated,
                    Term = groupedEvaluation.Key.Term,
                    EndDateTime = firstEvaluation.EndDateTime,
                    StartDateTime = firstEvaluation.StartDateTime,
                    Finished = groupedEvaluation.Count(evaluation => evaluation.Status == EvaluationStatus.Finished),
                    Total = groupedEvaluation.Count()
                });
            }

            return evaluations;
        }

        public Task<EvaluationResultDetailsDto> GetEvaluationResultDetail(long evaluationTemplateId)
        {
            throw new System.NotImplementedException();
        }

        public Task<EvaluationsComparisonDto> GetEvaluationComparision(EvaluationsComparisonInputDto input)
        {
            throw new System.NotImplementedException();
        }
    }
}