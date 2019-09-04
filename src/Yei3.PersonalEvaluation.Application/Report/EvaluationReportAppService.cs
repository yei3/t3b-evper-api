using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Castle.Core.Internal;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Application.Report.Dto;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.Report.Dto;

namespace Yei3.PersonalEvaluation.Report
{
    public class EvaluationReportAppService : ApplicationService, IEvaluationReportAppService
    {
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Evaluations.Sections.Section, long> _sectionRepository;
        private readonly IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> NotEvaluableQuestionRepository;
        private readonly IRepository<Evaluations.EvaluationQuestions.EvaluationMeasuredQuestion, long> _measuredQuestionRepository;
        private readonly UserManager UserManager;
        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> OrganizationUnitsRepository;

        private readonly ILogger _logger;

        public EvaluationReportAppService(IRepository<Evaluation, long> evaluationRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<Evaluations.Sections.Section, long> sectionRepository, IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> notEvaluableQuestionRepository, IRepository<EvaluationMeasuredQuestion, long> measuredQuestionRepository, UserManager userManager, IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitsRepository, ILogger logger)
        {
            EvaluationRepository = evaluationRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _sectionRepository = sectionRepository;
            NotEvaluableQuestionRepository = notEvaluableQuestionRepository;
            _measuredQuestionRepository = measuredQuestionRepository;
            UserManager = userManager;
            OrganizationUnitsRepository = organizationUnitsRepository;
            _logger = logger;
        }

        public Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesReport(long? period = null)
        {
            long userId = AbpSession.GetUserId();
            List<long> evaluationIds = new List<long>();

            evaluationIds = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .OrderByDescending(evaluation => evaluation.CreationTime)
                .Select(evaluation => evaluation.Id)
                .Take(2)
                .ToList();

            long lastEvaluationId = evaluationIds.LastOrDefault();
            long currentEvaluationId = evaluationIds.FirstOrDefault();

            if (lastEvaluationId == currentEvaluationId)
            {
                return Task.FromResult(
                    new CollaboratorObjectivesReportDto
                    {
                        PreviousTotal = 0,
                        PreviousValidated = 0,
                        CurrentTotal = NotEvaluableQuestionRepository
                            .GetAll()
                            .Include(question => question.Section)
                            .ThenInclude(section => section.ParentSection)
                            .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                            .Where(question => question.EvaluationId == currentEvaluationId)
                            .Count(question =>
                                question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                            ),
                        CurrentValidated = NotEvaluableQuestionRepository
                            .GetAll()
                            .Include(question => question.Section)
                            .ThenInclude(section => section.ParentSection)
                            .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                            .Where(question => question.EvaluationId == currentEvaluationId)
                            .Where(question => question.Status == EvaluationQuestionStatus.Validated)
                            .Count(question =>
                                question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                            )
                    }
                );
            }

            return Task.FromResult(
                new CollaboratorObjectivesReportDto
                {
                    PreviousTotal = NotEvaluableQuestionRepository
                        .GetAll()
                        .Include(question => question.Section)
                        .ThenInclude(section => section.ParentSection)
                        .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                        .Where(question => question.EvaluationId == lastEvaluationId)
                        .Count(question =>
                            question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                        ),
                    PreviousValidated = NotEvaluableQuestionRepository
                        .GetAll()
                        .Include(question => question.Section)
                        .ThenInclude(section => section.ParentSection)
                        .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                        .Where(question => question.EvaluationId == lastEvaluationId)
                        .Where(question => question.Status == EvaluationQuestionStatus.Validated)
                        .Count(question =>
                            question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                        ),
                    CurrentTotal = NotEvaluableQuestionRepository
                        .GetAll()
                        .Include(question => question.Section)
                        .ThenInclude(section => section.ParentSection)
                        .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                        .Where(question => question.EvaluationId == currentEvaluationId)
                        .Count(question =>
                            question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                        ),
                    CurrentValidated = NotEvaluableQuestionRepository
                        .GetAll()
                        .Include(question => question.Section)
                        .ThenInclude(section => section.ParentSection)
                        .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                        .Where(question => question.EvaluationId == currentEvaluationId)
                        .Where(question => question.Status == EvaluationQuestionStatus.Validated)
                        .Count(question =>
                            question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                        ),
                }
            );
        }

        public Task<IList<CapabilitiesReportDto>> GetCollaboratorCompetencesReport(long? period = null)
        {
            long evaluationId = 0;
            long evaluationTemplateId = 0;
            long userId = AbpSession.GetUserId();
            Evaluation firstEvaluation = null;
            Evaluation secondEvaluation = null;
            List<Evaluation> evaluations = new List<Evaluation>();

            // This must be refactor to a DTO to improve the code ;) @luiarhs
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

            evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .OrderByDescending(evaluation => evaluation.CreationTime)
                .Take(2)
                .ToList();

            firstEvaluation = evaluations.FirstOrDefault();
            secondEvaluation = evaluations.LastOrDefault();

            if (period.HasValue && period == 1)
            {
                if (firstEvaluation.Id == secondEvaluation.Id)
                {
                    return Task.FromResult(result);
                }
                else
                {
                    evaluationId = secondEvaluation.Id;
                    evaluationTemplateId = secondEvaluation.EvaluationId;
                }
            }
            else
            {
                evaluationId = firstEvaluation.Id;
                evaluationTemplateId = firstEvaluation.EvaluationId;
            }

            Evaluations.Sections.Section sections = _sectionRepository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .ThenInclude(question => question.EvaluationUnmeasuredQuestions)
                .ThenInclude(evaluationQuestion => evaluationQuestion.UnmeasuredAnswer)
                .Where(section => section.Name.StartsWith(AppConsts.SectionCapability, StringComparison.CurrentCultureIgnoreCase))
                .Where(section => evaluationTemplateId == section.EvaluationTemplateId)
                .FirstOrDefault();

            foreach (Evaluations.Sections.Section subSection in sections.ChildSections)
            {
                var Unsatisfactory = subSection?.UnmeasuredQuestions
                    .Select(uq =>
                        new
                        {
                            Value = uq.EvaluationUnmeasuredQuestions
                                .Where(euq => evaluationId == euq.EvaluationId)
                                .Where(euq => euq?.UnmeasuredAnswer?.Text == "-70").Count()
                        }.Value
                    ).ToList();

                var Satisfactory = subSection?.UnmeasuredQuestions
                    .Select(uq =>
                        new
                        {
                            Value = uq.EvaluationUnmeasuredQuestions
                                .Where(euq => evaluationId == euq.EvaluationId)
                                .Where(euq => euq?.UnmeasuredAnswer?.Text == "71-99").Count()
                        }.Value
                    ).ToList();

                var Exceeds = subSection?.UnmeasuredQuestions
                    .Select(uq =>
                        new
                        {
                            Value = uq.EvaluationUnmeasuredQuestions
                                .Where(euq => evaluationId == euq.EvaluationId)
                                .Where(euq => euq?.UnmeasuredAnswer?.Text == "+100").Count()
                        }.Value
                    ).ToList();

                foreach (var capabilities in result)
                {
                    if (subSection.Name.StartsWith(capabilities.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        for (int j = 0; j < Exceeds.Count; j++)
                        {
                            capabilities.Unsatisfactory += Unsatisfactory[j];
                            capabilities.Satisfactory += Satisfactory[j];
                            capabilities.Exceeds += Exceeds[j];
                        }
                        break;
                    }
                }
            }

            return Task.FromResult(result);
        }

        public Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesSalesReport(long? period = null)
        {
            long userId = AbpSession.GetUserId();
            List<long> evaluationIds = new List<long>();

            evaluationIds = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .OrderByDescending(evaluation => evaluation.CreationTime)
                .Select(evaluation => evaluation.Id)
                .Take(2)
                .ToList();

            long lastEvaluationId = evaluationIds.LastOrDefault();
            long currentEvaluationId = evaluationIds.FirstOrDefault();

            if (lastEvaluationId == currentEvaluationId)
            {
                return
                    Task.FromResult(new CollaboratorObjectivesReportDto
                    {
                        PreviousTotal = 0,
                        PreviousValidated = 0,
                        CurrentTotal = _measuredQuestionRepository
                            .GetAll()
                            .Count(question => question.EvaluationId == currentEvaluationId),
                        CurrentValidated = _measuredQuestionRepository
                            .GetAll()
                            .Include(question => question.MeasuredQuestion)
                            .Include(question => question.MeasuredAnswer)
                            .Where(question => question.EvaluationId == currentEvaluationId)
                            .Count(question => IsObjectiveAccomplished(question.MeasuredQuestion, question.MeasuredAnswer, question))
                    });
            }

            return Task.FromResult(
                new CollaboratorObjectivesReportDto
                {
                    PreviousTotal = _measuredQuestionRepository
                        .GetAll()
                        .Count(question => question.EvaluationId == lastEvaluationId),
                    PreviousValidated = _measuredQuestionRepository
                        .GetAll()
                        .Include(question => question.MeasuredQuestion)
                        .Include(question => question.MeasuredAnswer)
                        .Where(question => question.EvaluationId == lastEvaluationId)
                        .Count(question => IsObjectiveAccomplished(question.MeasuredQuestion, question.MeasuredAnswer, question)),
                    CurrentTotal = _measuredQuestionRepository
                        .GetAll()
                        .Count(question => question.EvaluationId == currentEvaluationId),
                    CurrentValidated = _measuredQuestionRepository
                        .GetAll()
                        .Include(question => question.MeasuredQuestion)
                        .Include(question => question.MeasuredAnswer)
                        .Where(question => question.EvaluationId == currentEvaluationId)
                        .Count(question => IsObjectiveAccomplished(question.MeasuredQuestion, question.MeasuredAnswer, question))
                }
            );
        }

        public Task<IList<SalesCapabilitiesReportDto>> GetCollaboratorCapabilitiesSalesReport(long? period = null)
        {
            long evaluationId = 0;
            long evaluationTemplateId = 0;
            long userId = AbpSession.GetUserId();
            Evaluation firstEvaluation = null;
            Evaluation secondEvaluation = null;
            List<Evaluation> evaluations = new List<Evaluation>();

            IList<SalesCapabilitiesReportDto> result =
                new List<SalesCapabilitiesReportDto>() {

                    new SalesCapabilitiesReportDto {
                        Name = "Competencias del puesto",
                        Total = 0,
                        Satisfactory = 0,
                    },
                    new SalesCapabilitiesReportDto {
                        Name = "Cultura 3B",
                        Total = 0,
                        Satisfactory = 0,
                    }
                };

            evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .OrderByDescending(evaluation => evaluation.CreationTime)
                .Take(2)
                .ToList();

            firstEvaluation = evaluations.FirstOrDefault();
            secondEvaluation = evaluations.LastOrDefault();

            if (period.HasValue && period == 1)
            {
                if (firstEvaluation.Id == secondEvaluation.Id)
                {
                    return Task.FromResult(result);
                }
                else
                {
                    evaluationId = secondEvaluation.Id;
                    evaluationTemplateId = secondEvaluation.EvaluationId;
                }
            }
            else
            {
                evaluationId = firstEvaluation.Id;
                evaluationTemplateId = firstEvaluation.EvaluationId;
            }

            IList<Evaluations.Sections.Section> sections = _sectionRepository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .ThenInclude(question => question.EvaluationUnmeasuredQuestions)
                .ThenInclude(evaluationQuestion => evaluationQuestion.UnmeasuredAnswer)
                .Where(section => evaluationTemplateId == section.EvaluationTemplateId)
                .Where(
                    section =>
                        section.Name.StartsWith(AppConsts.Section3bCulture, StringComparison.CurrentCultureIgnoreCase)
                        ||
                        section.Name.StartsWith(AppConsts.SectionJobCapability, StringComparison.CurrentCultureIgnoreCase)
                    )
                .Take(2)
                .ToList();

            foreach (var section in sections)
            {
                foreach (Evaluations.Sections.Section subSection in section.ChildSections)
                {
                    var Unsatisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationId == euq.EvaluationId)
                                    .Where(euq => euq?.UnmeasuredAnswer?.Action == "false").Count()
                            }.Value
                        ).ToList();

                    var Satisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationId == euq.EvaluationId)
                                    .Where(euq => euq?.UnmeasuredAnswer?.Action == "true").Count()
                            }.Value
                        ).ToList();

                    foreach (var capabilities in result)
                    {
                        if (section.Name.StartsWith(capabilities.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            capabilities.Total = subSection.UnmeasuredQuestions.Count;
                            for (int j = 0; j < Satisfactory.Count; j++)
                            {
                                capabilities.Satisfactory += Satisfactory[j];
                            }
                            break;
                        }
                    }
                }
            }

            return Task.FromResult(result);
        }

        public async Task<AdministratorObjectiveReportDto> GetEvaluatorObjectivesReport(AdministratorInputDto input)
        {
            User evaluatorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .WhereIf(input.StartTime != null, evaluation => evaluation.CreationTime >= input.StartTime)
                .WhereIf(input.EndDateTime != null, evaluation => evaluation.CreationTime <= input.EndDateTime)
                .AsQueryable();

            List<long> evaluationIds = new List<long>();

            Abp.Organizations.OrganizationUnit currentOrganizationUnit = null;

            if (input.UserId.HasValue)
            {
                evaluationIds = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value)
                    .Select(evaluation => evaluation.Id)
                    .ToList();
            }
            else
            {
                long? organizationUnitId = 0;
                List<long> userIds = (await UserManager.GetSubordinatesTree(evaluatorUser))
                    .Select(user => user.Id)
                    .ToList();

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                if (organizationUnitId.HasValue && input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId == organizationUnit.Id)
                            .First();

                    userIds = userIds
                        .Where(userId => UserManager.IsInOrganizationUnitAsync(userId, currentOrganizationUnit.Id).GetAwaiter().GetResult())
                        .ToList();

                    //* Only add the evaluator if it belongs to the same area
                    if (currentOrganizationUnit.DisplayName.Contains(evaluatorUser.Area))
                    {
                        userIds.Add(evaluatorUser.Id);
                    }
                }
                else if (organizationUnitId.HasValue && !input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        await OrganizationUnitsRepository
                            .SingleAsync(organizationUnit => organizationUnitId == organizationUnit.Id);

                    userIds = (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true))
                        .Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }

                evaluationIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.Id)
                    .ToList();
            }

            return new AdministratorObjectiveReportDto
            {
                TotalObjectives = NotEvaluableQuestionRepository
                    .GetAll()
                    .Include(question => question.Section)
                    .ThenInclude(section => section.ParentSection)
                    .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                    .Where(question => evaluationIds.Contains(question.EvaluationId))
                    .Count(question =>
                        question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                    ),
                ValidatedObjectives = NotEvaluableQuestionRepository
                    .GetAll()
                    .Include(question => question.Section)
                    .ThenInclude(section => section.ParentSection)
                    .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                    .Where(question => evaluationIds.Contains(question.EvaluationId))
                    .Where(question => question.Status == EvaluationQuestionStatus.Validated)
                    .Count(question =>
                        question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                    ),
            };
        }

        public async Task<IList<CapabilitiesReportDto>> GetEvaluatorCapabilitiesReport(AdministratorInputDto input)
        {
            User evaluatorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.CreationTime >= input.StartTime)
                .Where(evaluation => evaluation.CreationTime <= input.EndDateTime);

            List<long> evaluationIds = new List<long>();
            List<long> templateIds = new List<long>();

            Abp.Organizations.OrganizationUnit currentOrganizationUnit = null;

            if (input.UserId.HasValue)
            {
                evaluations = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value);

                evaluationIds = evaluations
                    .Select(evaluation => evaluation.Id)
                    .ToList();

                templateIds = evaluations
                    .Select(evaluation => evaluation.EvaluationId)
                    .ToList();
            }
            else
            {
                long? organizationUnitId = 0;
                List<long> userIds = (await UserManager.GetSubordinatesTree(evaluatorUser))
                            .Select(user => user.Id)
                            .ToList();

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                if (organizationUnitId.HasValue && input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId.Equals(organizationUnit.Id))
                            .First();

                    userIds = userIds
                        .Where(userId => UserManager.IsInOrganizationUnitAsync(userId, currentOrganizationUnit.Id).GetAwaiter().GetResult())
                        .ToList();

                    //* Only add the evaluator if it belongs to the same area
                    if (currentOrganizationUnit.DisplayName.Contains(evaluatorUser.Area))
                    {
                        userIds.Add(evaluatorUser.Id);
                    }
                }
                else if (organizationUnitId.HasValue && !input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        await OrganizationUnitsRepository.SingleAsync(organizationUnit =>
                            organizationUnitId.Equals(organizationUnit.Id));

                    userIds = (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true))
                        .Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }

                evaluationIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.Id)
                    .ToList();

                templateIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.EvaluationId)
                    .Distinct()
                    .ToList();
            }

            List<Evaluations.Sections.Section> sections = _sectionRepository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .ThenInclude(question => question.EvaluationUnmeasuredQuestions)
                .ThenInclude(evaluationQuestion => evaluationQuestion.UnmeasuredAnswer)
                .Where(section => section.Name.StartsWith(AppConsts.SectionCapability, StringComparison.CurrentCultureIgnoreCase))
                .Where(section => templateIds.Contains(section.EvaluationTemplateId))
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
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "-70").Count()
                            }.Value
                        ).ToList();

                    var Satisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "71-99").Count()
                            }.Value
                        ).ToList();

                    var Exceeds = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
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
                            }
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public async Task<AdministratorObjectiveReportDto> GetEvaluatorObjectivesSalesReport(AdministratorInputDto input)
        {
            User evaluatorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .WhereIf(input.StartTime != null, evaluation => evaluation.CreationTime >= input.StartTime)
                .WhereIf(input.EndDateTime != null, evaluation => evaluation.CreationTime <= input.EndDateTime)
                .AsQueryable();

            List<long> evaluationIds = new List<long>();

            Abp.Organizations.OrganizationUnit currentOrganizationUnit = null;

            if (input.UserId.HasValue)
            {
                evaluationIds = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value)
                    .Select(evaluation => evaluation.Id)
                    .ToList();
            }
            else
            {
                long? organizationUnitId = 0;
                List<long> userIds = (await UserManager.GetSubordinatesTree(evaluatorUser))
                    .Select(user => user.Id)
                    .ToList();

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                if (organizationUnitId.HasValue && input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId == organizationUnit.Id)
                            .First();

                    userIds = userIds
                        .Where(userId => UserManager.IsInOrganizationUnitAsync(userId, currentOrganizationUnit.Id).GetAwaiter().GetResult())
                        .ToList();

                    //* Only add the evaluator if it belongs to the same area
                    if (currentOrganizationUnit.DisplayName.Contains(evaluatorUser.Area))
                    {
                        userIds.Add(evaluatorUser.Id);
                    }
                }
                else if (organizationUnitId.HasValue && !input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        await OrganizationUnitsRepository
                            .SingleAsync(organizationUnit => organizationUnitId == organizationUnit.Id);

                    userIds = (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true))
                        .Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }

                evaluationIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.Id)
                    .ToList();
            }

            return new AdministratorObjectiveReportDto
            {
                TotalObjectives = _measuredQuestionRepository
                    .GetAll()
                    .Count(question => evaluationIds.Contains(question.EvaluationId)),
                ValidatedObjectives = _measuredQuestionRepository
                    .GetAll()
                    .Include(question => question.MeasuredQuestion)
                    .Include(question => question.MeasuredAnswer)
                    .Where(question => evaluationIds.Contains(question.EvaluationId))
                    .Count(question => IsObjectiveAccomplished(question.MeasuredQuestion, question.MeasuredAnswer, question))
            };
        }
        public async Task<IList<SalesCapabilitiesReportDto>> GetEvaluatorCapabilitiesSalesReport(AdministratorInputDto input)
        {
            User evaluatorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .WhereIf(input.StartTime != null, evaluation => evaluation.CreationTime >= input.StartTime)
                .WhereIf(input.EndDateTime != null, evaluation => evaluation.CreationTime <= input.EndDateTime)
                .AsQueryable();

            List<long> templateIds = new List<long>();
            List<long> evaluationIds = new List<long>();


            Abp.Organizations.OrganizationUnit currentOrganizationUnit = null;

            if (input.UserId.HasValue)
            {
                evaluationIds = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value)
                    .Select(evaluation => evaluation.Id)
                    .ToList();
            }
            else
            {
                long? organizationUnitId = 0;
                List<long> userIds = (await UserManager.GetSubordinatesTree(evaluatorUser))
                    .Select(user => user.Id)
                    .ToList();

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                if (organizationUnitId.HasValue && input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId == organizationUnit.Id)
                            .First();

                    userIds = userIds
                        .Where(userId => UserManager.IsInOrganizationUnitAsync(userId, currentOrganizationUnit.Id).GetAwaiter().GetResult())
                        .ToList();

                    //* Only add the evaluator if it belongs to the same area
                    if (currentOrganizationUnit.DisplayName.Contains(evaluatorUser.Area))
                    {
                        userIds.Add(evaluatorUser.Id);
                    }
                }
                else if (organizationUnitId.HasValue && !input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        await OrganizationUnitsRepository
                            .SingleAsync(organizationUnit => organizationUnitId == organizationUnit.Id);

                    userIds = (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true))
                        .Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }

                evaluationIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.Id)
                    .ToList();
            }

            templateIds = evaluations
                .Where(evaluation => evaluationIds.Contains(evaluation.Id))
                .Select(evaluation => evaluation.EvaluationId)
                .Distinct()
                .ToList();

            IQueryable<Evaluations.Sections.Section> sections = _sectionRepository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .ThenInclude(question => question.EvaluationUnmeasuredQuestions)
                .ThenInclude(evaluationQuestion => evaluationQuestion.UnmeasuredAnswer)
                .Where(section => templateIds.Contains(section.EvaluationTemplateId))
                .Where(
                    section =>
                        section.Name.StartsWith(AppConsts.Section3bCulture, StringComparison.CurrentCultureIgnoreCase)
                            || section.Name.StartsWith(AppConsts.SectionJobCapability, StringComparison.CurrentCultureIgnoreCase)
                    );

            List<SalesCapabilitiesReportDto> result = new List<SalesCapabilitiesReportDto>()
                {
                    new SalesCapabilitiesReportDto {
                        Name = "Competencias del puesto",
                        Total = 0,
                        Satisfactory = 0,
                    },
                    new SalesCapabilitiesReportDto {
                        Name = "Cultura 3B",
                        Total = 0,
                        Satisfactory = 0,
                    }
                };

            foreach (var section in sections)
            {
                foreach (Evaluations.Sections.Section subSection in section.ChildSections)
                {
                    var Total = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId)).Count()
                            }.Value
                        ).ToList();

                    var Satisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Action == "true").Count()
                            }.Value
                        ).ToList();

                    foreach (var capabilities in result)
                    {
                        if (section.Name.StartsWith(capabilities.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            capabilities.Total += Total.Count * Total[0];
                            for (int j = 0; j < Satisfactory.Count; j++)
                            {
                                capabilities.Satisfactory += Satisfactory[j];
                            }
                            break;
                        }
                    }
                }
            }

            return result;
        }
        public async Task<AdministratorObjectiveReportDto> GetAdministratorObjectivesReport(AdministratorInputDto input)
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.CreationTime >= input.StartTime)
                .Where(evaluation => evaluation.CreationTime <= input.EndDateTime);

            List<long> evaluationIds = new List<long>();

            if (input.UserId.HasValue)
            {
                evaluationIds = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value)
                    .Select(evaluation => evaluation.Id)
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

            return new AdministratorObjectiveReportDto
            {
                TotalObjectives = NotEvaluableQuestionRepository
                    .GetAll()
                    .Include(question => question.Section)
                    .ThenInclude(section => section.ParentSection)
                    .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                    .Where(question => evaluationIds.Contains(question.EvaluationId))
                    .Count(question =>
                        question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                    ),
                ValidatedObjectives = NotEvaluableQuestionRepository
                    .GetAll()
                    .Include(question => question.Section)
                    .ThenInclude(section => section.ParentSection)
                    .Where(question => question.Section.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                    .Where(question => evaluationIds.Contains(question.EvaluationId))
                    .Where(question => question.Status == EvaluationQuestionStatus.Validated)
                    .Count(question =>
                        question.Section.ParentSection.Name.Equals(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase)
                    )
            };
        }

        public async Task<IList<CapabilitiesReportDto>> GetAdministratorCapabilitiesReport(AdministratorInputDto input)
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.CreationTime >= input.StartTime)
                .Where(evaluation => evaluation.CreationTime <= input.EndDateTime);

            List<long> evaluationIds = new List<long>();
            List<long> templateIds = new List<long>();

            if (input.UserId.HasValue)
            {
                evaluations = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value);

                evaluationIds = evaluations
                    .Select(evaluation => evaluation.Id)
                    .ToList();

                templateIds = evaluations
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
                    Abp.Organizations.OrganizationUnit currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId == organizationUnit.Id)
                            .First();

                    userIds.AddRange(
                        (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true))
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

                templateIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.EvaluationId)
                    .Distinct()
                    .ToList();
            }

            List<Evaluations.Sections.Section> sections = _sectionRepository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .ThenInclude(question => question.EvaluationUnmeasuredQuestions)
                .ThenInclude(evaluationQuestion => evaluationQuestion.UnmeasuredAnswer)
                .Where(section => section.Name.StartsWith(AppConsts.SectionCapability, StringComparison.CurrentCultureIgnoreCase))
                .Where(section => templateIds.Contains(section.EvaluationTemplateId))
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
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "-70").Count()
                            }.Value
                        ).ToList();

                    var Satisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Text == "71-99").Count()
                            }.Value
                        ).ToList();

                    var Exceeds = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
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
                            }
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public async Task<AdministratorObjectiveReportDto> GetAdministratorObjectivesSalesReport(AdministratorInputDto input)
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.CreationTime >= input.StartTime)
                .Where(evaluation => evaluation.CreationTime <= input.EndDateTime);

            List<long> evaluationIds = new List<long>();

            if (input.UserId.HasValue)
            {
                evaluationIds = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value)
                    .Select(evaluation => evaluation.Id)
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
            return new AdministratorObjectiveReportDto
            {
                TotalObjectives = _measuredQuestionRepository
                    .GetAll()
                    .Count(question => evaluationIds.Contains(question.EvaluationId)),
                ValidatedObjectives = _measuredQuestionRepository
                    .GetAll()
                    .Include(question => question.MeasuredQuestion)
                    .Include(question => question.MeasuredAnswer)
                    .Where(question => evaluationIds.Contains(question.EvaluationId))
                    .Count(question => IsObjectiveAccomplished(question.MeasuredQuestion, question.MeasuredAnswer, question))
            };
        }

        public async Task<IList<SalesCapabilitiesReportDto>> GetAdministratorCapabilitiesSalesReport(AdministratorInputDto input)
        {User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.CreationTime >= input.StartTime)
                .Where(evaluation => evaluation.CreationTime <= input.EndDateTime);

            List<long> evaluationIds = new List<long>();
            List<long> templateIds = new List<long>();

            if (input.UserId.HasValue)
            {
                evaluations = evaluations
                    .Where(evaluation => evaluation.UserId == input.UserId.Value);

                evaluationIds = evaluations
                    .Select(evaluation => evaluation.Id)
                    .ToList();

                templateIds = evaluations
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
                    Abp.Organizations.OrganizationUnit currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId == organizationUnit.Id)
                            .First();

                    userIds.AddRange(
                        (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true))
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

                templateIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.EvaluationId)
                    .Distinct()
                    .ToList();
            }

            IQueryable<Evaluations.Sections.Section> sections = _sectionRepository
                .GetAll()
                .Include(section => section.ChildSections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .ThenInclude(question => question.EvaluationUnmeasuredQuestions)
                .ThenInclude(evaluationQuestion => evaluationQuestion.UnmeasuredAnswer)
                .Where(section => templateIds.Contains(section.EvaluationTemplateId))
                .Where(
                    section =>
                        section.Name.StartsWith(AppConsts.Section3bCulture, StringComparison.CurrentCultureIgnoreCase)
                            || section.Name.StartsWith(AppConsts.SectionJobCapability, StringComparison.CurrentCultureIgnoreCase)
                    );

            List<SalesCapabilitiesReportDto> result = new List<SalesCapabilitiesReportDto>()
                {
                    new SalesCapabilitiesReportDto {
                        Name = "Competencias del puesto",
                        Total = 0,
                        Satisfactory = 0,
                    },
                    new SalesCapabilitiesReportDto {
                        Name = "Cultura 3B",
                        Total = 0,
                        Satisfactory = 0,
                    }
                };

            foreach (var section in sections)
            {
                foreach (Evaluations.Sections.Section subSection in section.ChildSections)
                {
                    var Total = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId)).Count()
                            }.Value
                        ).ToList();

                    var Satisfactory = subSection?.UnmeasuredQuestions
                        .Select(uq =>
                            new
                            {
                                Value = uq.EvaluationUnmeasuredQuestions
                                    .Where(euq => evaluationIds.Contains(euq.EvaluationId))
                                    .Where(euq => euq?.UnmeasuredAnswer?.Action == "true").Count()
                            }.Value
                        ).ToList();

                    foreach (var capabilities in result)
                    {
                        if (section.Name.StartsWith(capabilities.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            capabilities.Total += Total.Count * Total[0];
                            for (int j = 0; j < Satisfactory.Count; j++)
                            {
                                capabilities.Satisfactory += Satisfactory[j];
                            }
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public async Task<EvaluationEmployeeDataDto> GetEvaluationEmployeeData(AdministratorInputDto input)
        {
            double seniorityAverage = 0;
            User evaluatorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.User)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .WhereIf(input.StartTime != null, evaluation => evaluation.CreationTime >= input.StartTime)
                .WhereIf(input.EndDateTime != null, evaluation => evaluation.CreationTime <= input.EndDateTime)
                .AsQueryable();

            List<User> users = new List<User>();
            List<long> evaluationIds = new List<long>();
            Abp.Organizations.OrganizationUnit currentOrganizationUnit = null;

            if (input.UserId.HasValue)
            {
                evaluations = evaluations.Where(evaluation => evaluation.UserId == input.UserId.Value);

                CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete);

                User currentUser = UserManager
                    .Users
                    .WhereIf(input.StartTime != null, user => user.EntryDate <= input.StartTime)
                    .WhereIf(input.EndDateTime != null, user => user.DeletionTime.HasValue ? user.DeletionTime >= input.EndDateTime : true)
                    .Single(user => user.Id == input.UserId.Value);

                CurrentUnitOfWork.EnableFilter(AbpDataFilters.SoftDelete);

                users.Add(currentUser);
            }
            else
            {
                long? organizationUnitId = 0;
                List<long> userIds = null;

                CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete);

                users = (await UserManager.GetSubordinatesTree(evaluatorUser))
                    .WhereIf(input.StartTime != null, user => user.EntryDate <= input.StartTime)
                    .WhereIf(input.EndDateTime != null, user => user.DeletionTime.HasValue ? user.DeletionTime >= input.EndDateTime : true)
                    .ToList();

                CurrentUnitOfWork.EnableFilter(AbpDataFilters.SoftDelete);

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                if (organizationUnitId.HasValue && input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId == organizationUnit.Id)
                            .First();

                    CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete);

                    users = users
                        .Where(user => UserManager.IsInOrganizationUnitAsync(user.Id, currentOrganizationUnit.Id).GetAwaiter().GetResult())
                        .WhereIf(input.StartTime != null, user => user.EntryDate <= input.StartTime)
                        .WhereIf(input.EndDateTime != null, user => user.DeletionTime.HasValue ? user.DeletionTime >= input.EndDateTime : true)
                        .ToList();

                    CurrentUnitOfWork.EnableFilter(AbpDataFilters.SoftDelete);

                    //* Only add the evaluator if it belongs to the same area
                    if (currentOrganizationUnit.DisplayName.Contains(evaluatorUser.Area))
                    {
                        users.Add(evaluatorUser);
                    }

                    userIds = users.Select(user => user.Id).ToList();
                }
                else if (organizationUnitId.HasValue && !input.JobDescription.IsNullOrEmpty())
                {
                    currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId == organizationUnit.Id)
                            .First();

                    CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete);

                    userIds = (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true))
                        .Where(user => user.JobDescription == input.JobDescription)
                        .WhereIf(input.StartTime != null, user => user.EntryDate <= input.StartTime)
                        .WhereIf(input.EndDateTime != null, user => user.DeletionTime.HasValue ? user.DeletionTime >= input.EndDateTime : true)
                        .Select(user => user.Id)
                        .ToList();

                    CurrentUnitOfWork.EnableFilter(AbpDataFilters.SoftDelete);
                }

                evaluations = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId));
            }

            try
            {
                seniorityAverage = users
                    .WhereIf(!input.JobDescription.IsNullOrEmpty(), user => user.JobDescription == input.JobDescription)
                    .Select(user => user.EntryDate)
                    .ToList()
                    .Select(entryDate => (DateTime.Now - entryDate).TotalDays)
                    .Average() / AppConsts.YearsLengthInDays;
            }
            catch (InvalidOperationException)
            {
                // do nothing
            }

            return new EvaluationEmployeeDataDto
            {
                TotalEmployees = users
                    .Where(user => user.Id != evaluatorUser.Id)
                    .WhereIf(!input.JobDescription.IsNullOrEmpty(), user => user.JobDescription == input.JobDescription)
                    .ToList().Count,
                EvaluatedEmployees = evaluations
                    .Where(evaluation => evaluation.Status != EvaluationStatus.NonInitiated)
                    .Select(evaluation => evaluation.UserId)
                    .Distinct()
                    .Count(),
                SeniorityAverage = seniorityAverage
            };
        }

        public async Task<EvaluationEmployeeDataDto> GetAdministratorEvaluationEmployeeData(AdministratorInputDto input)
        {
            double seniorityAverage = 0;

            IQueryable<Evaluation> evaluations = EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.User)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .WhereIf(input.StartTime != null, evaluation => evaluation.CreationTime >= input.StartTime)
                .WhereIf(input.EndDateTime != null, evaluation => evaluation.CreationTime <= input.EndDateTime)
                .AsQueryable();

            List<User> users = new List<User>();
            List<long> evaluationIds = new List<long>();
            Abp.Organizations.OrganizationUnit currentOrganizationUnit = null;

            if (input.UserId.HasValue)
            {
                evaluations = evaluations.Where(evaluation => evaluation.UserId == input.UserId.Value);

                CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete);

                User currentUser = UserManager
                    .Users
                    .WhereIf(input.StartTime != null, user => user.EntryDate <= input.StartTime)
                    .WhereIf(input.EndDateTime != null, user => user.DeletionTime.HasValue ? user.DeletionTime >= input.EndDateTime : true)
                    .Single(user => user.Id == input.UserId.Value);

                users.Add(currentUser);

                CurrentUnitOfWork.EnableFilter(AbpDataFilters.SoftDelete);
            }
            else
            {
                long? organizationUnitId = 0;
                List<long> userIds = null;

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                currentOrganizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Single(organizationUnit => organizationUnitId == organizationUnit.Id);

                CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete);

                users = (await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit))
                    .WhereIf(input.StartTime != null, user => user.EntryDate <= input.StartTime)
                    .WhereIf(input.EndDateTime != null, user => user.DeletionTime.HasValue ? user.DeletionTime >= input.EndDateTime : true)
                    .ToList();

                CurrentUnitOfWork.EnableFilter(AbpDataFilters.SoftDelete);

                if (!input.JobDescription.IsNullOrEmpty())
                {
                    users = users
                        .Where(user => user.JobDescription == input.JobDescription)
                        .ToList();
                }

                userIds = users.Select(user => user.Id).ToList();

                evaluations = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId));
            }

            try
            {
                seniorityAverage = users
                    .WhereIf(!input.JobDescription.IsNullOrEmpty(), user => user.JobDescription == input.JobDescription)
                    .Select(user => user.EntryDate)
                    .ToList()
                    .Select(entryDate => (DateTime.Now - entryDate).TotalDays)
                    .Average() / AppConsts.YearsLengthInDays;
            }
            catch (InvalidOperationException)
            {
                // do nothing
            }

            return new EvaluationEmployeeDataDto
            {
                TotalEmployees = users
                    .ToList()
                    .Count,
                EvaluatedEmployees = evaluations
                    .Where(evaluation => evaluation.Status != EvaluationStatus.NonInitiated)
                    .Select(evaluation => evaluation.UserId)
                    .Distinct()
                    .Count(),
                SeniorityAverage = seniorityAverage
            };
        }

        protected bool IsObjectiveNumericAccomplished(decimal expected, decimal real, MeasuredQuestionRelation relation)
        {
            switch (relation)
            {
                case MeasuredQuestionRelation.Equals: return expected == real;
                case MeasuredQuestionRelation.Higher: return real > expected;
                case MeasuredQuestionRelation.HigherOrEquals: return real >= expected;
                case MeasuredQuestionRelation.Lower: return real < expected;
                case MeasuredQuestionRelation.LowerOrEquals: return real <= expected;
                case MeasuredQuestionRelation.Undefined: return false;
                default: return false;
            }
        }

        protected bool IsObjectiveAccomplished(MeasuredQuestion question, MeasuredAnswer answer, EvaluationMeasuredQuestion evaluationQuestion)
        {
            string expectedText = question.ExpectedText.IsNullOrEmpty()
                ? evaluationQuestion.ExpectedText
                : question.ExpectedText;

            decimal expectedValue = evaluationQuestion.Expected.HasValue
                ? evaluationQuestion.Expected.Value
                : question.Expected;

            return expectedText.IsNullOrEmpty()
                ? IsObjectiveNumericAccomplished(expectedValue, answer.Real, question.Relation)
                : question.ExpectedText == answer.Text;
        }

    }
}