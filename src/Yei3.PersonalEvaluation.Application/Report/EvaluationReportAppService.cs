using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Castle.Core.Internal;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.Evaluations.Terms;
using Yei3.PersonalEvaluation.Report.Dto;

namespace Yei3.PersonalEvaluation.Report
{
    public class EvaluationReportAppService : ApplicationService, IEvaluationReportAppService
    {
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Evaluations.Sections.Section, long> SectionRepository;
        private readonly IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> NotEvaluableQuestionRepository;
        private readonly UserManager UserManager;
        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> OrganizationUnitsRepository;

        public EvaluationReportAppService(IRepository<Evaluation, long> evaluationRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<Evaluations.Sections.Section, long> sectionRepository, UserManager userManager, IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> notEvaluableQuestionRepository, IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitsRepository)
        {
            EvaluationRepository = evaluationRepository;
            _unitOfWorkManager = unitOfWorkManager;
            SectionRepository = sectionRepository;
            UserManager = userManager;
            NotEvaluableQuestionRepository = notEvaluableQuestionRepository;
            OrganizationUnitsRepository = organizationUnitsRepository;
        }

        public Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesReport(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .OrderByDescending(evaluation => evaluation.CreationTime)
                .Take(2);

            Evaluation currentEvaluation = evaluations.FirstOrDefault();

            Evaluation lastEvaluation = evaluations.LastOrDefault();

            return Task.FromResult(
                new CollaboratorObjectivesReportDto
                {
                    PreviousTotal = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == "Objetivos")
                        .Count(question => question.EvaluationId == lastEvaluation.Id),
                    PreviousValidated = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.EvaluationId == lastEvaluation.Id)
                        .Where(question => question.Section.Name == "Objetivos")
                        .Count(question => question.Status == EvaluationQuestionStatus.Validated),
                    CurrentTotal = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == "Objetivos")
                        .Count(question => question.EvaluationId == currentEvaluation.Id),
                    CurrentValidated = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == "Objetivos")
                        .Where(question => question.EvaluationId == currentEvaluation.Id)
                        .Count(question => question.Status == EvaluationQuestionStatus.Validated)
                }
            );
        }

        public Task<IList<CapabilitiesReportDto>> GetCollaboratorCompetencesReport(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();
            List<long> evaluationIds = new List<long>();
            List<long> evaluationTemplatesIds = new List<long>();

            IQueryable<Evaluation> evaluations = EvaluationRepository
               .GetAll()
               .Where(evaluation => evaluation.UserId == userId)
               .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
               .OrderByDescending( evaluation => evaluation.CreationTime)
               .Take(2);            

            evaluationIds = evaluations
                    .Select(evaluation => evaluation.Id)
                    .ToList();
                
            evaluationTemplatesIds = evaluations
                .Select(evaluation => evaluation.EvaluationId)
                .ToList();

            List<Evaluations.Sections.Section> sections = SectionRepository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .ThenInclude(question => question.EvaluationUnmeasuredQuestions)
                .ThenInclude(evaluationQuestion => evaluationQuestion.UnmeasuredAnswer)
                .Where(section => section.Name.StartsWith(AppConsts.SectionCapability, StringComparison.CurrentCultureIgnoreCase))
                .Where(section => evaluationTemplatesIds.Contains(section.EvaluationTemplateId))
                .ToList();

            IList<CapabilitiesReportDto> result = 
                new List<CapabilitiesReportDto>() {
                    new CapabilitiesReportDto {
                        Name = "Orientación a resultados",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Eficiencia",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Orientación al detalle",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Comunicación",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Capacidad de análisis y solución de problemas",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Negociación",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Cultura 3B",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    }
                };
            
            foreach (Evaluations.Sections.Section section in sections)
            {   
                foreach (Evaluations.Sections.Section subSection in section.ChildSections)
                {                    
                    var Unsatisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "-70").Count()
                            }.Value
                        ).ToList();

                    var Satisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "71-99").Count()
                            }.Value
                        ).ToList();

                    var Exceeds = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "+100").Count()
                            }.Value
                        ).ToList();

                    //sorry for this too ¯\_(ツ)_/¯
                    foreach (var capabilitie in result)
                    {
                        if (subSection.Name.StartsWith(capabilitie.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            for (int j = 0; j < Exceeds.Count; j++)
                            {
                                capabilitie.Unsatisfactory += Unsatisfactory[j];
                                capabilitie.Satisfactory += Satisfactory[j];
                                capabilitie.Exceeds += Exceeds[j];
                            } break;
                        }
                    }
                }
            }

            return Task.FromResult(result);
        }

        [Obsolete]
        public async Task<ICollection<EvaluationResultsDto>> GetEvaluationResults()
        {
            var groupedEvaluations = EvaluationRepository
                .GetAll()
                .GroupBy(evaluation => new
                {
                    Id = evaluation.EvaluationId,
                    CreationTimeDayOfYear = evaluation.CreationTime.DayOfYear,
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
                    EvaluationTemplateId = groupedEvaluation.Key.Id,
                    CreationTime = firstEvaluation.CreationTime,
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

        [Obsolete]
        public async Task<ICollection<EvaluationResultsDto>> GetEvaluationCollaboratorResults()
        {
            IQueryable<EvaluationResultsDto> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == AbpSession.GetUserId())
                .OrderBy(evaluation => evaluation.CreationTime)
                .Select(evaluation => new EvaluationResultsDto
                {
                    Term = evaluation.Term,
                    Status = evaluation.Status,
                    Id = evaluation.Id,
                    CreationTime = evaluation.CreationTime,
                    EvaluationTemplateId = evaluation.EvaluationId,
                    EndDateTime = evaluation.EndDateTime,
                    StartDateTime = evaluation.StartDateTime,
                    Total = evaluation.Questions.OfType<EvaluationMeasuredQuestion>().Count(),
                    Finished = evaluation.Questions
                        .OfType<EvaluationMeasuredQuestion>()
                        .Where(evaluationMeasuredQuestion => evaluationMeasuredQuestion.IsActive)
                        .Count(evaluationMeasuredQuestion => evaluationMeasuredQuestion.Status == EvaluationQuestionStatus.Validated)
                })
                .Skip(0)
                .Take(2);

            return await evaluations.ToListAsync();
        }

        [Obsolete]
        public async Task<ICollection<EvaluationResultsDto>> GetEvaluationSupervisorResults()
        {
            User supervisorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            List<Evaluation> evaluationsResult = new List<Evaluation>();

            foreach (Abp.Organizations.OrganizationUnit organizationUnit in organizationUnits)
            {
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit))
                    .Distinct()
                    .Where(user =>
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Supervisor).GetAwaiter().GetResult()) &&
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Administrator).GetAwaiter().GetResult()))
                    .ToList();

                foreach (User user in users)
                {
                    evaluationsResult.AddRange(EvaluationRepository
                        .GetAll()
                        .Where(evaluation => evaluation.UserId == user.Id));
                }
            }

            var groupedEvaluations = evaluationsResult
                .AsQueryable()
                .OrderBy(evaluation => evaluation.CreationTime)
                .GroupBy(evaluation => new
                {
                    Id = evaluation.EvaluationId,
                    CreationTime = evaluation.CreationTime.DayOfYear,
                    StartTime = evaluation.StartDateTime,
                    EndTime = evaluation.EndDateTime,
                    CreatorUserId = evaluation.CreatorUserId,
                    Term = evaluation.Term
                })
                .Skip(0)
                .Take(2);

            List<EvaluationResultsDto> evaluations = new List<EvaluationResultsDto>();

            foreach (var groupedEvaluation in groupedEvaluations)
            {
                Evaluation firstEvaluation = groupedEvaluation.First();
                evaluations.Add(new EvaluationResultsDto()
                {
                    EvaluationTemplateId = groupedEvaluation.Key.Id,
                    Status = firstEvaluation.StartDateTime < DateTime.Now
                        ? EvaluationStatus.NonInitiated
                        : firstEvaluation.EndDateTime <= DateTime.Now
                            ? EvaluationStatus.Finished
                            : EvaluationStatus.NonInitiated,
                    Term = groupedEvaluation.Key.Term,
                    CreationTime = firstEvaluation.CreationTime,
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

        public async Task<EvaluationsComparisonDto> GetEvaluationComparision(EvaluationsComparisonInputDto input)
        {
            EvaluationsComparisonDto evaluationsComparison = new EvaluationsComparisonDto
            {
                LeftEvaluation = await GetEvaluationReport(input.LeftEvaluationTemplateId, input.LeftEvaluationTerm, input.LeftEvaluationDayOfYear),
                RightEvaluation = await GetEvaluationReport(input.RightEvaluationTemplateId, input.RightEvaluationTerm, input.RightEvaluationYear)
            };

            return await Task.FromResult(evaluationsComparison);
        }

        public async Task<EvaluationsComparisonDto> GetCollaboratorEvaluationComparision(UserEvaluationsComparisonInputDto input)
        {
            EvaluationsComparisonDto evaluationsComparison = new EvaluationsComparisonDto
            {
                LeftEvaluation = await GetEvaluationReport(input.LeftEvaluationTemplateId, input.LeftEvaluationTerm, input.LeftEvaluationDayOfYear, AbpSession.UserId),
                RightEvaluation = await GetEvaluationReport(input.RightEvaluationTemplateId, input.RightEvaluationTerm, input.RightEvaluationYear, AbpSession.UserId)
            };

            return await Task.FromResult(evaluationsComparison);
        }

        public async Task<ICollection<EvaluationsCollaboratorComparisonDto>> GetSupervisorEvaluationComparision(UserEvaluationsComparisonInputDto input)
        {
            User supervisorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            List<EvaluationsCollaboratorComparisonDto> collaboratorComparisons = new List<EvaluationsCollaboratorComparisonDto>();

            foreach (Abp.Organizations.OrganizationUnit organizationUnit in organizationUnits)
            {
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit))
                    .Distinct()
                    .Where(user =>
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Supervisor).GetAwaiter().GetResult()) &&
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Administrator).GetAwaiter().GetResult()))
                    .ToList();

                foreach (User user in users)
                {
                    collaboratorComparisons.Add(new EvaluationsCollaboratorComparisonDto
                    {
                        LeftEvaluation = await GetEvaluationReport(input.LeftEvaluationTemplateId, input.LeftEvaluationTerm, input.LeftEvaluationDayOfYear, user.Id),
                        RightEvaluation = await GetEvaluationReport(input.RightEvaluationTemplateId, input.RightEvaluationTerm, input.RightEvaluationYear, user.Id),
                        UserFullName = user.FullName
                    });
                }
            }

            return collaboratorComparisons;
        }

        public async Task<AdministratorObjectiveReportDto> GetAdministratorObjectivesReport(AdministratorInputDto input)
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            if (!await UserManager.IsInRoleAsync(administratorUser, StaticRoleNames.Tenants.Administrator))
            {
                throw new UserFriendlyException($"Usuario {administratorUser.FullName} no es un Administrador.");
            }

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.CreationTime >= input.StartTime)
                .Where(evaluation => evaluation.CreationTime <= input.EndDateTime);

            List<long> evaluationIds = new List<long>();

            if (input.UserId.HasValue)
            {
                evaluations = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value);
            }
            else
            {
                long? organizationUnitId = 0;
                List<long> userIds = new List<long>();                

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                if (organizationUnitId.HasValue && input.JobDescription.IsNullOrEmpty())
                {
                    Abp.Organizations.OrganizationUnit _organizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId.Equals(organizationUnit.Id))
                            .First();

                        userIds.AddRange(
                            (await UserManager.GetUsersInOrganizationUnit(_organizationUnit, true))
                            .Select(user => user.Id)
                            .ToList());
                }
                else if (organizationUnitId.HasValue && !input.JobDescription.IsNullOrEmpty())
                {
                    Abp.Organizations.OrganizationUnit areaOrganizationUnit =
                        await OrganizationUnitsRepository.SingleAsync(organizationUnit =>
                            organizationUnitId.Equals(organizationUnit.Id));

                    userIds = (await UserManager.GetUsersInOrganizationUnit(areaOrganizationUnit, true))
                        .Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }
                else if (!input.JobDescription.IsNullOrEmpty() && organizationUnitId.HasValue)
                {
                    userIds = UserManager.Users.Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }

                evaluationIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.Id)
                    .ToList();
            }
            // harcoded SeniorityAverage
            Random rd = new Random();

            return new AdministratorObjectiveReportDto
            {
                TotalObjectives = NotEvaluableQuestionRepository
                    .GetAll()
                    .Where(question => question.Section.Name == AppConsts.SectionObjectivesName)
                    .Count(question => evaluationIds.Contains(question.EvaluationId)),
                ValidatedObjectives = NotEvaluableQuestionRepository
                    .GetAll()
                    .Where(question => question.Section.Name == AppConsts.SectionObjectivesName)
                    .Where(question => evaluationIds.Contains(question.EvaluationId))
                    .Count(question => question.Status == EvaluationQuestionStatus.Validated),
                SeniorityAverage = rd.Next(0,5),
            };
        }

        public async Task<IList<CapabilitiesReportDto>> GetAdministratorCapabilitiesReport(AdministratorInputDto input)
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            if (!await UserManager.IsInRoleAsync(administratorUser, StaticRoleNames.Tenants.Administrator))
            {
                throw new UserFriendlyException($"Usuario {administratorUser.FullName} no es un Administrador.");
            }

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.CreationTime >= input.StartTime)
                .Where(evaluation => evaluation.CreationTime <= input.EndDateTime);

            List<long> evaluationIds = new List<long>();
            List<long> evaluationTemplatesIds = new List<long>();

            if (input.UserId.HasValue)
            {
                evaluations = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value);

                evaluationIds = evaluations
                    .Select(evaluation => evaluation.Id)
                    .ToList();
                
                evaluationTemplatesIds = evaluations
                    .Select(evaluation => evaluation.EvaluationId)
                    .ToList();
            }
            else
            {
                long? organizationUnitId = 0;
                List<long> userIds = new List<long>();

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                if (organizationUnitId.HasValue && input.JobDescription.IsNullOrEmpty())
                {
                    Abp.Organizations.OrganizationUnit _organizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId.Equals(organizationUnit.Id))
                            .First();

                        userIds.AddRange(
                            (await UserManager.GetUsersInOrganizationUnit(_organizationUnit, true))
                            .Select(user => user.Id)
                            .ToList());
                }
                else if (organizationUnitId.HasValue && !input.JobDescription.IsNullOrEmpty())
                {
                    Abp.Organizations.OrganizationUnit areaOrganizationUnit =
                        await OrganizationUnitsRepository.SingleAsync(organizationUnit =>
                            organizationUnitId.Equals(organizationUnit.Id));

                    userIds = (await UserManager.GetUsersInOrganizationUnit(areaOrganizationUnit, true))
                        .Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }
                else if (!organizationUnitId.HasValue && !input.JobDescription.IsNullOrEmpty())
                {
                    userIds = UserManager.Users.Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }

                evaluationIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.Id)
                    .ToList();

                evaluationTemplatesIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.EvaluationId)
                    .Distinct()
                    .ToList();
            }

            List<Evaluations.Sections.Section> sections = SectionRepository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .ThenInclude(question => question.EvaluationUnmeasuredQuestions)
                .ThenInclude(evaluationQuestion => evaluationQuestion.UnmeasuredAnswer)
                .Where(section => section.Name.StartsWith(AppConsts.SectionCapability, StringComparison.CurrentCultureIgnoreCase))
                .Where(section => evaluationTemplatesIds.Contains(section.EvaluationTemplateId))
                .ToList();
                
            // I'm sorry for this
            IList<CapabilitiesReportDto> result = 
                new List<CapabilitiesReportDto>() {
                    new CapabilitiesReportDto {
                        Name = "Orientación a resultados",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Eficiencia",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Orientación al detalle",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Comunicación",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Capacidad de análisis y solución de problemas",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Negociación",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    },
                    new CapabilitiesReportDto {
                        Name = "Cultura 3B",
                        Unsatisfactory = 0,
                        Satisfactory = 0,
                        Exceeds = 0,
                    }
                };
            
            foreach (Evaluations.Sections.Section section in sections)
            {   
                foreach (Evaluations.Sections.Section subSection in section.ChildSections)
                {                    
                    var Unsatisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "-70").Count()
                            }.Value
                        ).ToList();

                    var Satisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "71-99").Count()
                            }.Value
                        ).ToList();

                    var Exceeds = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "+100").Count()
                            }.Value
                        ).ToList();

                    //sorry for this too ¯\_(ツ)_/¯
                    foreach (var capabilitie in result)
                    {
                        if (subSection.Name.StartsWith(capabilitie.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            for (int j = 0; j < Exceeds.Count; j++)
                            {
                                capabilitie.Unsatisfactory += Unsatisfactory[j];
                                capabilitie.Satisfactory += Satisfactory[j];
                                capabilitie.Exceeds += Exceeds[j];
                            } break;
                        }
                    }
                }
            }

            return result;
        }

        protected bool IsObjectiveCompleted(MeasuredAnswer answer, MeasuredQuestion question)
        {
            if (answer == null)
            {
                return false;
            }

            if (!answer.Text.IsNullOrEmpty())
            {
                return answer.Text == question.ExpectedText;
            }

            switch (question.Relation)
            {
                case MeasuredQuestionRelation.Equals: return answer.Real == question.Expected;
                case MeasuredQuestionRelation.Higher: return answer.Real > question.Expected;
                case MeasuredQuestionRelation.HigherOrEquals: return answer.Real >= question.Expected;
                case MeasuredQuestionRelation.Lower: return answer.Real < question.Expected;
                case MeasuredQuestionRelation.LowerOrEquals: return answer.Real <= question.Expected;
                default: return false;
            }
        }

        protected async Task<EvaluationReportDto> GetEvaluationReport(long evaluationTemplateId, EvaluationTerm term, int evaluationDayOfYear, long? userId = null)
        {
            var groupedEvaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.EvaluationId == evaluationTemplateId)
                .Where(evaluation => evaluation.Term == term)
                .Where(evaluation => evaluation.CreationTime.DayOfYear == evaluationDayOfYear)
                .OrderBy(evaluation => evaluation.CreationTime)
                .Include(evaluation => evaluation.User)
                .Include(evaluation => evaluation.Template)
                .ThenInclude(template => template.Sections)
                .WhereIf(userId.HasValue, evaluation => evaluation.UserId == userId.Value)
                .GroupBy(evaluation => new
                {
                    EvaluationTemplateId = evaluation.EvaluationId,
                    CreationTime = evaluation.CreationTime.DayOfYear,
                    StartTime = evaluation.StartDateTime,
                    EndTime = evaluation.EndDateTime,
                    CreatorUserId = evaluation.CreatorUserId,
                    Term = evaluation.Term
                });

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

            var firstGroupedEvaluation = groupedEvaluations.First();


            EvaluationReportDto reportDto = new EvaluationReportDto
            {
                Term = firstGroupedEvaluation.Key.Term,
                CreationTime = firstGroupedEvaluation.First().CreationTime,
                AntiquityAverage = firstGroupedEvaluation.Sum(evaluation => DateTime.Today.Year - evaluation.User.EntryDate.Year) /
                                   firstGroupedEvaluation.Count(),
                EvaluatedEmployees = firstGroupedEvaluation.Count(),
                Sections = SectionRepository
                    .GetAll()
                    .Where(section => section.EvaluationTemplateId == firstGroupedEvaluation.Key.EvaluationTemplateId)
                    .Where(section => !section.MeasuredQuestions.IsNullOrEmpty() || !section.UnmeasuredQuestions.IsNullOrEmpty())
                    .Where(section => section.Name.StartsWith(AppConsts.SectionCapability, StringComparison.CurrentCultureIgnoreCase))
                    .Select(section => new SectionSummaryDto
                    {
                        Id = section.Id,
                        Name = section.Name
                    }).ToList(),
                Name = firstGroupedEvaluation.First().Name,
                TotalEmployees = totalEmployees
            };

            foreach (SectionSummaryDto reportSection in reportDto.Sections)
            {
                reportSection.FinishedQuestions = (await SectionRepository
                        .GetAll()
                        .Include(section => section.MeasuredQuestions)
                        .ThenInclude(measuredQuestion => measuredQuestion.EvaluationMeasuredQuestions)
                        .SingleAsync(section => section.Id == reportSection.Id))
                    .MeasuredQuestions
                    .Select(measuredQuestion =>
                        measuredQuestion.EvaluationMeasuredQuestions
                            .WhereIf(userId.HasValue, evaluationMeasuredQuestion => evaluationMeasuredQuestion.Evaluation.UserId == userId.Value)
                            .Count(evaluationMeasuredQuestion =>
                            evaluationMeasuredQuestion.Status == EvaluationQuestionStatus.Validated &&
                            (IsObjectiveCompleted(evaluationMeasuredQuestion.MeasuredAnswer, evaluationMeasuredQuestion.MeasuredQuestion))))
                    .Sum();

                reportSection.FinishedQuestions += (await SectionRepository
                        .GetAll()
                        .Include(section => section.UnmeasuredQuestions)
                        .ThenInclude(measuredQuestion => measuredQuestion.EvaluationUnmeasuredQuestions)
                        .SingleAsync(section => section.Id == reportSection.Id))
                    .UnmeasuredQuestions
                    .Select(unmeasuredQuestion =>
                        unmeasuredQuestion.EvaluationUnmeasuredQuestions
                            .WhereIf(userId.HasValue, evaluationUnmeasuredQuestion => evaluationUnmeasuredQuestion.Evaluation.UserId == userId.Value)
                            .Count(evaluationUnmeasuredQuestion =>
                            evaluationUnmeasuredQuestion.Status == EvaluationQuestionStatus.Validated))
                    .Sum();

                reportSection.NonAnsweredQuestions = (await SectionRepository
                        .GetAll()
                        .Include(section => section.MeasuredQuestions)
                        .ThenInclude(measuredQuestion => measuredQuestion.EvaluationMeasuredQuestions)
                        .SingleAsync(section => section.Id == reportSection.Id))
                    .MeasuredQuestions
                    .Select(measuredQuestion =>
                        measuredQuestion.EvaluationMeasuredQuestions
                            .WhereIf(userId.HasValue, evaluationMeasuredQuestion => evaluationMeasuredQuestion.Evaluation.UserId == userId.Value)
                            .Count(evaluationMeasuredQuestion =>
                            evaluationMeasuredQuestion.Status == EvaluationQuestionStatus.Unanswered ||
                            evaluationMeasuredQuestion.Status == EvaluationQuestionStatus.NoStatus))
                    .Sum();

                reportSection.NonAnsweredQuestions += (await SectionRepository
                        .GetAll()
                        .Include(section => section.UnmeasuredQuestions)
                        .ThenInclude(measuredQuestion => measuredQuestion.EvaluationUnmeasuredQuestions)
                        .SingleAsync(section => section.Id == reportSection.Id))
                    .UnmeasuredQuestions
                    .Select(unmeasuredQuestion =>
                        unmeasuredQuestion.EvaluationUnmeasuredQuestions
                            .WhereIf(userId.HasValue, evaluationUnmeasuredQuestion => evaluationUnmeasuredQuestion.Evaluation.UserId == userId.Value)
                            .Count(evaluationUnmeasuredQuestion =>
                            evaluationUnmeasuredQuestion.Status == EvaluationQuestionStatus.Unanswered ||
                            evaluationUnmeasuredQuestion.Status == EvaluationQuestionStatus.NoStatus))
                    .Sum();

                reportSection.NonFinishedQuestions = (await SectionRepository
                        .GetAll()
                        .Include(section => section.MeasuredQuestions)
                        .ThenInclude(measuredQuestion => measuredQuestion.EvaluationMeasuredQuestions)
                        .SingleAsync(section => section.Id == reportSection.Id))
                    .MeasuredQuestions
                    .Select(measuredQuestion =>
                        measuredQuestion.EvaluationMeasuredQuestions
                            .WhereIf(userId.HasValue, evaluationMeasuredQuestion => evaluationMeasuredQuestion.Evaluation.UserId == userId.Value)
                            .Count(evaluationMeasuredQuestion =>
                            (evaluationMeasuredQuestion.Status == EvaluationQuestionStatus.Answered &&
                             IsObjectiveCompleted(evaluationMeasuredQuestion.MeasuredAnswer, evaluationMeasuredQuestion.MeasuredQuestion)) ||
                            (evaluationMeasuredQuestion.Status == EvaluationQuestionStatus.Answered ||
                             evaluationMeasuredQuestion.Status == EvaluationQuestionStatus.Validated) &&
                            !IsObjectiveCompleted(evaluationMeasuredQuestion.MeasuredAnswer, evaluationMeasuredQuestion.MeasuredQuestion)))
                    .Sum();

                reportSection.NonFinishedQuestions += (await SectionRepository
                        .GetAll()
                        .Include(section => section.UnmeasuredQuestions)
                        .ThenInclude(measuredQuestion => measuredQuestion.EvaluationUnmeasuredQuestions)
                        .SingleAsync(section => section.Id == reportSection.Id))
                    .UnmeasuredQuestions
                    .Select(unmeasuredQuestion =>
                        unmeasuredQuestion.EvaluationUnmeasuredQuestions
                            .WhereIf(userId.HasValue, evaluationUnmeasuredQuestion => evaluationUnmeasuredQuestion.Evaluation.UserId == userId.Value)
                            .Count(evaluationUnmeasuredQuestion =>
                            evaluationUnmeasuredQuestion.Status == EvaluationQuestionStatus.Answered))
                    .Sum();
            }

            return await Task.FromResult(reportDto);
        }

        public async Task<ICollection<EvaluationResultsDto>> GetEvaluationCollaboratorResultsById(UserEvaluationResultDto input)
        {
            IQueryable<EvaluationResultsDto> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == input.UserId)
                .OrderBy(evaluation => evaluation.CreationTime)
                .Select(evaluation => new EvaluationResultsDto
                {
                    Term = evaluation.Term,
                    Status = evaluation.Status,
                    Id = evaluation.Id,
                    CreationTime = evaluation.CreationTime,
                    EvaluationTemplateId = evaluation.EvaluationId,
                    EndDateTime = evaluation.EndDateTime,
                    StartDateTime = evaluation.StartDateTime,
                    Total = evaluation.Questions.OfType<EvaluationMeasuredQuestion>().Count(),
                    Finished = evaluation.Questions
                        .OfType<EvaluationMeasuredQuestion>()
                        .Where(evaluationMeasuredQuestion => evaluationMeasuredQuestion.IsActive)
                        .Count(evaluationMeasuredQuestion => evaluationMeasuredQuestion.Status == EvaluationQuestionStatus.Validated)
                })
                .Skip(0)
                .Take(2);

            return await evaluations.ToListAsync();
        }

        public Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesReportById(UserEvaluationResultDto input)
        {
            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == input.UserId)
                .OrderByDescending(evaluation => evaluation.CreationTime)
                .Take(2);

            Evaluation currentEvaluation = evaluations.FirstOrDefault();

            Evaluation lastEvaluation = evaluations.LastOrDefault();

            return Task.FromResult(
                new CollaboratorObjectivesReportDto
                {
                    PreviousTotal = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == "Objetivos")
                        .Count(question => question.EvaluationId == lastEvaluation.Id),
                    PreviousValidated = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.EvaluationId == lastEvaluation.Id)
                        .Where(question => question.Section.Name == "Objetivos")
                        .Count(question => question.Status == EvaluationQuestionStatus.Validated),
                    CurrentTotal = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == "Objetivos")
                        .Count(question => question.EvaluationId == currentEvaluation.Id),
                    CurrentValidated = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == "Objetivos")
                        .Where(question => question.EvaluationId == currentEvaluation.Id)
                        .Count(question => question.Status == EvaluationQuestionStatus.Validated)
                }
            );
        }
    }
}