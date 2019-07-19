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
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<Evaluations.Sections.Section, long> SectionRepository;
        private readonly IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> NotEvaluableQuestionRepository;
        private readonly IRepository<Evaluations.EvaluationQuestions.EvaluationMeasuredQuestion, long> MeasuredQuestionRepository;
        private readonly UserManager UserManager;
        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> OrganizationUnitsRepository;

        public EvaluationReportAppService(
            IRepository<Evaluation, long> evaluationRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Evaluations.Sections.Section, long> sectionRepository,
            UserManager userManager,
            IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> notEvaluableQuestionRepository,
            IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitsRepository,
            IRepository<Evaluations.EvaluationQuestions.EvaluationMeasuredQuestion, long> measuredQuestionRepository)
        {
            EvaluationRepository = evaluationRepository;
            _unitOfWorkManager = unitOfWorkManager;
            SectionRepository = sectionRepository;
            UserManager = userManager;
            NotEvaluableQuestionRepository = notEvaluableQuestionRepository;
            OrganizationUnitsRepository = organizationUnitsRepository;
            MeasuredQuestionRepository = measuredQuestionRepository;
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
                            .Where(question => question.Section.Name == AppConsts.SectionObjectivesName)
                            .Count(question => question.EvaluationId == currentEvaluationId),
                        CurrentValidated = NotEvaluableQuestionRepository
                            .GetAll()
                            .Where(question => question.Section.Name == AppConsts.SectionObjectivesName)
                            .Where(question => question.EvaluationId == currentEvaluationId)
                            .Count(question => question.Status == EvaluationQuestionStatus.Validated)
                    }
                );
            }

            return Task.FromResult(
                new CollaboratorObjectivesReportDto
                {
                    PreviousTotal = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == AppConsts.SectionObjectivesName)
                        .Count(question => question.EvaluationId == lastEvaluationId),
                    PreviousValidated = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.EvaluationId == lastEvaluationId)
                        .Where(question => question.Section.Name == AppConsts.SectionObjectivesName)
                        .Count(question => question.Status == EvaluationQuestionStatus.Validated),
                    CurrentTotal = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == AppConsts.SectionObjectivesName)
                        .Count(question => question.EvaluationId == currentEvaluationId),
                    CurrentValidated = NotEvaluableQuestionRepository
                        .GetAll()
                        .Where(question => question.Section.Name == AppConsts.SectionObjectivesName)
                        .Where(question => question.EvaluationId == currentEvaluationId)
                        .Count(question => question.Status == EvaluationQuestionStatus.Validated)
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

            Evaluations.Sections.Section sections = SectionRepository
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
            // todo fix hardcoded SeniorityAverage
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
                SeniorityAverage = rd.Next(0, 5),
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

        public Task<CollaboratorObjectivesReportDto> GetCollaboratorObjectivesAccomplishmentReport(long? period = null)
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
                        CurrentTotal = MeasuredQuestionRepository
                            .GetAll()
                            .Count(question => question.EvaluationId == currentEvaluationId),
                        CurrentValidated = MeasuredQuestionRepository
                            .GetAll()
                            .Include(question => question.MeasuredQuestion)
                            .Include(question => question.MeasuredAnswer)
                            .Where(question => question.EvaluationId == currentEvaluationId)
                            .Count(question => IsObjectiveAccomplished(question.MeasuredQuestion, question.MeasuredAnswer))
                    });
            }

            return Task.FromResult(
                new CollaboratorObjectivesReportDto
                {
                    PreviousTotal = MeasuredQuestionRepository
                        .GetAll()
                        .Count(question => question.EvaluationId == lastEvaluationId),
                    PreviousValidated = MeasuredQuestionRepository
                        .GetAll()
                        .Include(question => question.MeasuredQuestion)
                        .Include(question => question.MeasuredAnswer)
                        .Where(question => question.EvaluationId == lastEvaluationId)
                        .Count(question => IsObjectiveAccomplished(question.MeasuredQuestion, question.MeasuredAnswer)),
                    CurrentTotal = MeasuredQuestionRepository
                        .GetAll()
                        .Count(question => question.EvaluationId == currentEvaluationId),
                    CurrentValidated = MeasuredQuestionRepository
                        .GetAll()
                        .Include(question => question.MeasuredQuestion)
                        .Include(question => question.MeasuredAnswer)
                        .Where(question => question.EvaluationId == currentEvaluationId)
                        .Count(question => IsObjectiveAccomplished(question.MeasuredQuestion, question.MeasuredAnswer))
                }
            );
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

        protected bool IsObjectiveAccomplished(MeasuredQuestion question, MeasuredAnswer answer)
        {
            return question.ExpectedText.IsNullOrEmpty()
                ? IsObjectiveNumericAccomplished(question.Expected, answer.Real, question.Relation)
                : question.ExpectedText == answer.Text;
        }

        public Task<IList<SalesCapabilitiesReportDto>> GetCollaboratorAccomplishmentReport(long? period = null)
        {
            long evaluationId = 0;
            long evaluationTemplateId = 0;
            long userId = AbpSession.GetUserId();
            Evaluation firstEvaluation = null;
            Evaluation secondEvaluation = null;
            List<Evaluation> evaluations = new List<Evaluation>();

            // This must be refactor to a DTO to improve the code ;) @luiarhs
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

            IList<Evaluations.Sections.Section> sections = SectionRepository
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
                    Abp.Organizations.OrganizationUnit _organizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId.Equals(organizationUnit.Id))
                            .First();
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
                    userIds = (await UserManager.GetSubordinatesTree(evaluatorUser))
                        .Where(user => user.JobDescription == input.JobDescription)
                        .Select(user => user.Id)
                        .ToList();
                }

                evaluationIds = evaluations
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Select(evaluation => evaluation.Id)
                    .ToList();
            }
            // todo fix hardcoded SeniorityAverage
            Random rd = new Random();

            return new AdministratorObjectiveReportDto
            {
                TotalObjectives = NotEvaluableQuestionRepository
                    .GetAll()
                    .Where(question => question.Section.Name.Contains(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                    .Count(question => evaluationIds.Contains(question.EvaluationId)),
                ValidatedObjectives = NotEvaluableQuestionRepository
                    .GetAll()
                    .Where(question => question.Section.Name.Contains(AppConsts.SectionObjectivesName, StringComparison.InvariantCultureIgnoreCase))
                    .Where(question => evaluationIds.Contains(question.EvaluationId))
                    .Count(question => question.Status == EvaluationQuestionStatus.Validated),
                SeniorityAverage = rd.Next(0, 5),
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
                List<long> userIds = (await UserManager.GetSubordinatesTree(evaluatorUser))
                            .Select(user => user.Id)
                            .ToList();

                organizationUnitId = (input.AreaId.HasValue && input.AreaId != AppConsts.Zero) ? input.AreaId : input.RegionId;

                if (organizationUnitId.HasValue && input.JobDescription.IsNullOrEmpty())
                {
                    Abp.Organizations.OrganizationUnit _organizationUnit =
                        OrganizationUnitsRepository
                            .GetAll()
                            .Where(organizationUnit => organizationUnitId.Equals(organizationUnit.Id))
                            .First();
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
                    userIds = (await UserManager.GetSubordinatesTree(evaluatorUser))
                        .Where(user => user.JobDescription == input.JobDescription)
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
    }
}