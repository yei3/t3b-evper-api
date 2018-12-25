using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Castle.Core.Internal;
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
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EvaluationReportAppService(IRepository<Evaluation, long> evaluationRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            EvaluationRepository = evaluationRepository;
            _unitOfWorkManager = unitOfWorkManager;
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

            List<EvaluationResultsDto> evaluations = new List<EvaluationResultsDto>();

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

            return await Task.FromResult(evaluations);
        }

        public async Task<EvaluationResultDetailsDto> GetEvaluationResultDetail(long evaluationTemplateId)
        {

            var groupedEvaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.EvaluationId == evaluationTemplateId)
                .OrderBy(evaluation => evaluation.CreationTime)
                .Include(evaluation => evaluation.User)
                .Include(evaluation => evaluation.Template)
                .GroupBy(evaluation => new
                {
                    EvaluationTemplateId = evaluation.EvaluationId,
                    CreationTime = evaluation.CreationTime.DayOfYear,
                    StartTime = evaluation.StartDateTime,
                    EndTime = evaluation.EndDateTime,
                    CreatorUserId = evaluation.CreatorUserId,
                    Term = evaluation.Term
                })
                .Skip(0)
                .Take(2);

            int totalEmployees = 0;

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {

                totalEmployees = EvaluationRepository
                    .GetAll()
                    .Where(evaluation => evaluation.EvaluationId == evaluationTemplateId)
                    .OrderBy(evaluation => evaluation.CreationTime)
                    .Include(evaluation => evaluation.User)
                    .Include(evaluation => evaluation.Template)
                    .GroupBy(evaluation => new
                    {
                        EvaluationTemplateId = evaluation.EvaluationId,
                        CreationTime = evaluation.CreationTime.DayOfYear,
                        StartTime = evaluation.StartDateTime,
                        EndTime = evaluation.EndDateTime,
                        CreatorUserId = evaluation.CreatorUserId,
                        Term = evaluation.Term
                    })
                    .First()
                    .Count();
            }

            var firstEvaluation = groupedEvaluations.First();
            var previousEvaluation = groupedEvaluations.Last();

            EvaluationResultDetailsDto evaluationDetails;

            switch (groupedEvaluations.Count())
            {
                case 1:
                    evaluationDetails = new EvaluationResultDetailsDto()
                    {
                        Term = firstEvaluation.Key.Term,
                        CreationTime = firstEvaluation.First().CreationTime,
                        AntiquityAverage = firstEvaluation.Sum(evaluation => DateTime.Today.Year - evaluation.User.EntryDate.Year) /
                                           firstEvaluation.Count(),
                        EvaluatedEmployees = firstEvaluation.Count(),
                        EvaluationDescription = firstEvaluation.First().Template.Description,
                        EvaluationName = firstEvaluation.First().Name,
                        FinishedEvaluations =
                            firstEvaluation.Count(evaluation => evaluation.Status == EvaluationStatus.Finished),
                        PreviousEvaluationCreationTime = new DateTime(0, 0, 0),
                        PreviousEvaluationEvaluatedEmployees = 0,
                        PreviousEvaluationFinishedEvaluations = 0,
                        PreviousEvaluationTerm = 0,
                        TotalEmployees = totalEmployees
                    };
                    break;
                case 2:
                    evaluationDetails = new EvaluationResultDetailsDto()
                    {
                        Term = firstEvaluation.Key.Term,
                        CreationTime = firstEvaluation.First().CreationTime,
                        AntiquityAverage = firstEvaluation.Sum(evaluation => DateTime.Today.Year - evaluation.User.EntryDate.Year) /
                                           firstEvaluation.Count(),
                        EvaluatedEmployees = firstEvaluation.Count(),
                        EvaluationDescription = firstEvaluation.First().Template.Description,
                        EvaluationName = firstEvaluation.First().Name,
                        FinishedEvaluations =
                            firstEvaluation.Count(evaluation => evaluation.Status == EvaluationStatus.Finished),
                        PreviousEvaluationCreationTime = previousEvaluation.First().CreationTime,
                        PreviousEvaluationEvaluatedEmployees = previousEvaluation.Count(),
                        PreviousEvaluationFinishedEvaluations = previousEvaluation.Count(evaluation => evaluation.Status == EvaluationStatus.Finished),
                        PreviousEvaluationTerm = previousEvaluation.Key.Term,
                        TotalEmployees = totalEmployees
                    };
                    break;
                default:
                    throw new UserFriendlyException(404, "No se encuentran las evalauciones requeridas");
            }


            return await Task.FromResult(evaluationDetails);
        }

        public Task<EvaluationsComparisonDto> GetEvaluationComparision(EvaluationsComparisonInputDto input)
        {
            throw new System.NotImplementedException();
        }
    }
}