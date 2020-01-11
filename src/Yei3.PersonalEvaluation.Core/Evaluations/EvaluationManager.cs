using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Terms;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;

namespace Yei3.PersonalEvaluation.Evaluations
{

    public class EvaluationManager : DomainService, IEvaluationManager
    {

        private readonly IAbpSession AbpSession;
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<EvaluationQuestion, long> EvaluationQuestionRepository;
        private readonly UserManager UserManager;

        public EvaluationManager(IAbpSession abpSession, IRepository<Evaluation, long> evaluationRepository, IRepository<EvaluationQuestion, long> evaluationQuestionRepository, UserManager userManager)
        {
            AbpSession = abpSession;
            EvaluationRepository = evaluationRepository;
            EvaluationQuestionRepository = evaluationQuestionRepository;
            UserManager = userManager;
        }

        public async Task<EvaluationTerm> GetUserNextEvaluationTermAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            var nextEvaluation = await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.Status == EvaluationStatus.NonInitiated)
                .OrderBy(evaluation => evaluation.CreationTime)
                .FirstOrDefaultAsync(evaluation => evaluation.UserId == userId);

            return nextEvaluation == null ? EvaluationTerm.NoTerm : nextEvaluation.Term;
        }

        public async Task<ToDoesSummaryValueObject> GetUserToDoesSummary(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            return new ToDoesSummaryValueObject
            {
                Evaluations = await GetUserPendingEvaluationsCountAsync(userId),
                AutoEvaluations = await GetUserPendingAutoEvaluationsCountAsync(userId),
                Objectives = await GetUserPendingObjectivesCountAsync(userId)
            };
        }

        public async Task<int> GetUserPendingAutoEvaluationsCountAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.NonInitiated)
                .Where(evaluation => evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.EndDateTime.AddMonths(1) > DateTime.Now)
                .CountAsync();
        }

        public async Task<List<EvaluationSummaryValueObject>> GetUserPendingEvaluationsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User user = await UserManager.GetUserByIdAsync(userId.Value);

            return await EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.Template)
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.EndDateTime.AddMonths(1) > DateTime.Now)
                .Where(evaluation => evaluation.Status != EvaluationStatus.PendingReview)
                .Where(evaluation => evaluation.Status != EvaluationStatus.Validated)
                .Select(evaluation => new EvaluationSummaryValueObject
                {
                    Term = evaluation.Term,
                    Id = evaluation.Id,
                    Name = evaluation.Name,
                    Description = evaluation.Template.Description,
                    Status = evaluation.Status,
                    EndDateTime = evaluation.EndDateTime,
                    CollaboratorName = user.FullName,
                    IsAutoEvaluation = evaluation.Template.IsAutoEvaluation
                }).ToListAsync();
        }

        public async Task<List<RevisionSummaryValueObject>> GetUserPendingEvaluationRevisionsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.PendingReview)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Include(evaluation => evaluation.Template)
                .Include(evaluation => evaluation.Revision)
                .Select(evaluation => new RevisionSummaryValueObject
                {
                    EvaluationId = evaluation.Id,
                    Term = evaluation.Term,
                    Status = evaluation.Status,
                    EndDateTime = evaluation.EndDateTime,
                    Name = evaluation.Name,
                    Description = evaluation.Template.Description,
                    RevisionDateTime = evaluation.Revision.RevisionDateTime,
                    CollaboratorFullName = evaluation.User.FullName,
                    IsAutoEvaluation = evaluation.Template.IsAutoEvaluation
                }).ToListAsync();
        }

        public async Task<List<EvaluationObjectivesSummaryValueObject>> GetUserPendingObjectiveAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            Evaluation lastEvaluation = await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Template.IsAutoEvaluation)
                .OrderByDescending(evaluation => evaluation.Id)
                .FirstOrDefaultAsync();

            if (lastEvaluation == null)
            {
                return new List<EvaluationObjectivesSummaryValueObject>();
            }

            List<EvaluationObjectivesSummaryValueObject> evaluationObjectivesSummaryValueObjects = await EvaluationQuestionRepository
                .GetAll()
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .ThenInclude(evaluation => evaluation.Template)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.Template.IsAutoEvaluation)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.Id == lastEvaluation.Id)
                .OrderByDescending(evaluationQuestion => evaluationQuestion.Evaluation.Id)
                .OfType<NotEvaluableQuestion>()
                .Select(evaluationQuestion => new EvaluationObjectivesSummaryValueObject
                {
                    Status = evaluationQuestion.Status,
                    Name = evaluationQuestion.Text,
                    DeliveryDate = evaluationQuestion.NotEvaluableAnswer.CommitmentTime,
                    Id = evaluationQuestion.Id,
                    Binnacle = evaluationQuestion.Binnacle.Select(objectiveBinnacle => new ObjectiveBinnacleValueObject
                    {
                        Id = objectiveBinnacle.Id,
                        EvaluationQuestionId = objectiveBinnacle.EvaluationQuestionId,
                        Text = objectiveBinnacle.Text,
                        CreationTime = objectiveBinnacle.CreationTime,
                        CreatorUserId = objectiveBinnacle.CreatorUserId.Value
                    }).ToList(),
                    isNotEvaluable = true,
                    isNextObjective = evaluationQuestion.Section.ParentSection.Name == "Próximos Objetivos" ? true : false
                }).ToListAsync();

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                foreach (EvaluationObjectivesSummaryValueObject evaluationsSummary in evaluationObjectivesSummaryValueObjects)
                {
                    foreach (ObjectiveBinnacleValueObject binnacle in evaluationsSummary.Binnacle)
                    {
                        binnacle.UserName = UserManager.Users.Single(user => user.Id == binnacle.CreatorUserId).FullName;
                    }
                }
            }

            evaluationObjectivesSummaryValueObjects.AddRange(await EvaluationQuestionRepository
                .GetAll()
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .Where(evaluationQuestion => evaluationQuestion.Status != EvaluationQuestionStatus.Validated)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.Id == lastEvaluation.Id)
                .OfType<EvaluationMeasuredQuestion>()
                .Select(evaluationQuestion => new EvaluationObjectivesSummaryValueObject
                {
                    Status = evaluationQuestion.Status,
                    Name = evaluationQuestion.MeasuredQuestion.Text,
                    Deliverable = evaluationQuestion.MeasuredQuestion.Deliverable,
                    DeliveryDate = evaluationQuestion.TerminationDateTime,
                    Id = evaluationQuestion.Id,
                    Binnacle = evaluationQuestion.Binnacle.Select(objectiveBinnacle => new ObjectiveBinnacleValueObject
                    {
                        Id = objectiveBinnacle.Id,
                        EvaluationQuestionId = objectiveBinnacle.EvaluationQuestionId,
                        Text = objectiveBinnacle.Text,
                        CreationTime = objectiveBinnacle.CreationTime
                    }).ToList(),
                    isNotEvaluable = false
                }).ToListAsync());

            return evaluationObjectivesSummaryValueObjects
                .OrderBy(dashboard => dashboard.DeliveryDate)
                .ToList();
        }

        public async Task<List<EvaluationObjectivesSummaryValueObject>> GetUserObjectivesHome(long? userId = null)
        {
            List<EvaluationObjectivesSummaryValueObject> evaluationObjectivesSummaryValueObjects = await GetUserPendingObjectiveAsync(userId);
            return evaluationObjectivesSummaryValueObjects
                .OrderBy(dashboard => dashboard.DeliveryDate)
                .ToList();
        }

        public async Task<int> GetUserPendingEvaluationsCountAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.PendingReview)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.EndDateTime.AddMonths(1) > DateTime.Now)
                .CountAsync();
        }

        public Task<int> GetUserPendingObjectivesCountAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();
            int pendingObjectivesCountAsync = 0;

            IQueryable<Evaluation> pendingEvaluations = EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.Questions)
                .ThenInclude(question => ((NotEvaluableQuestion)question).Section)
                .Include(evaluation => evaluation.Template)
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.Status == EvaluationStatus.NonInitiated || evaluation.Status == EvaluationStatus.Pending)
                .Where(evaluation => evaluation.EndDateTime.AddMonths(1) > DateTime.Now);

            foreach (Evaluation pendingEvaluation in pendingEvaluations)
            {
                pendingObjectivesCountAsync += pendingEvaluation
                    .Questions
                    .OfType<EvaluationMeasuredQuestion>()
                    .Count(question => question.Status != EvaluationQuestionStatus.Validated);
                pendingObjectivesCountAsync += pendingEvaluation
                    .Questions
                    .OfType<NotEvaluableQuestion>()
                    .Where(question => question.Evaluation.Template.IsAutoEvaluation)
                    .Where(question => question.Section.Name != "Próximos Objetivos")
                    .Count(question => question.Status != EvaluationQuestionStatus.Validated);
            }

            return Task.FromResult(pendingObjectivesCountAsync);
        }

        public async Task<int> GetUserOrganizationUnitPendingEvaluationsCountAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);
            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            int pendingEvaluations = 0;

            foreach (Abp.Organizations.OrganizationUnit organizationUnit in organizationUnits)
            {
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit, true))
                    .Distinct()
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .ToList();
                foreach (User user in users)
                {
                    pendingEvaluations += await GetUserPendingEvaluationsCountAsync(user.Id);
                }
            }

            return pendingEvaluations;
        }

        public async Task<int> GetUserOrganizationUnitPendingEvaluationValidationsCountAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);
            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            int pendingRevisions = 0;

            foreach (Abp.Organizations.OrganizationUnit organizationUnit in organizationUnits)
            {
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit, true))
                    .Distinct()
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .ToList();

                foreach (User user in users)
                {
                    pendingRevisions += (await GetUserPendingObjectiveAsync(user.Id))
                        .Count(objective => objective.Status == EvaluationQuestionStatus.Answered);
                }
            }

            return pendingRevisions;
        }

        public async Task<ICollection<EvaluationSummaryValueObject>> GetUserOrganizationUnitCollaboratorsPendingEvaluationsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);

            List<EvaluationSummaryValueObject> evaluationsSummary = new List<EvaluationSummaryValueObject>();

            List<User> users = UserManager.Users
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .ToList();

            foreach (User user in users)
            {
                List<EvaluationSummaryValueObject> currentUserEvaluations = (await GetUserPendingEvaluationsAsync(user.Id))
                    .Where(evaluation => !evaluation.IsAutoEvaluation)
                    .ToList();
                evaluationsSummary.AddRange(currentUserEvaluations);
            }


            return evaluationsSummary;
        }

        public async Task<ICollection<RevisionSummaryValueObject>> GetUserOrganizationUnitPendingEvaluationRevisionsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);

            bool isSupervisor = await UserManager.IsInRoleAsync(supervisorUser, StaticRoleNames.Tenants.Supervisor);

            if (!isSupervisor)
            {
                throw new UserFriendlyException($"El usuario {supervisorUser.FullName} no autorizado.");
            }

            //List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            List<RevisionSummaryValueObject> revisionsSummary = new List<RevisionSummaryValueObject>();


            List<long> userIds = (await UserManager.GetSubordinatesTree(supervisorUser))
                .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                .Select(user => user.Id)
                .ToList();

            foreach (long idUser in userIds)
            {
                List<RevisionSummaryValueObject> currentUserPendingRevisions = (await GetUserPendingEvaluationRevisionsAsync(idUser))
                    .ToList();

                revisionsSummary.AddRange(currentUserPendingRevisions);
            }


            return revisionsSummary;
        }

        public async Task<ICollection<CollaboratorsPendingObjectivesSummaryValueObject>> GetUserOrganizationUnitObjectivesSummaryAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);

            bool isSupervisor = await UserManager.IsInRoleAsync(supervisorUser, StaticRoleNames.Tenants.Supervisor);

            if (!isSupervisor)
            {
                throw new UserFriendlyException($"El usuario {supervisorUser.FullName} no autorizado.");
            }

            List<CollaboratorsPendingObjectivesSummaryValueObject> collaboratorsPendingObjectivesSummary = new List<CollaboratorsPendingObjectivesSummaryValueObject>();

            List<User> users = UserManager.Users
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .ToList();

            foreach (User user in users)
            {
                List<EvaluationObjectivesSummaryValueObject> userObjectivesSummary = await GetUserObjectivesHome(user.Id);

                collaboratorsPendingObjectivesSummary.Add(new CollaboratorsPendingObjectivesSummaryValueObject
                {
                    CollaboratorFullName = user.FullName,
                    CollaboratorEmployeeNumber = user.EmployeeNumber,
                    ObjectivesSummary = userObjectivesSummary,
                    TotalPendingObjectives = userObjectivesSummary
                        .Where(objective => objective.Status != EvaluationQuestionStatus.Answered)
                        .Count(objective => objective.Status != EvaluationQuestionStatus.Validated),
                    AccomplishedObjectives = userObjectivesSummary
                        .Where(objective => objective.Status == EvaluationQuestionStatus.Answered)
                        .Count(objective => objective.Status == EvaluationQuestionStatus.Validated)
                });
            }


            return collaboratorsPendingObjectivesSummary;
        }

        public async Task<List<EvaluationSummaryValueObject>> GetUserEvaluationsHistoryAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User user = await UserManager.GetUserByIdAsync(userId.Value);

            List<EvaluationSummaryValueObject> evaluationsResult = await EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.Template)
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Status != EvaluationStatus.Pending)
                .Where(evaluation => evaluation.Status != EvaluationStatus.NonInitiated)
                .Where(evaluation => evaluation.Template.IsAutoEvaluation)
                .Select(evaluation => new EvaluationSummaryValueObject
                {
                    Term = evaluation.Term,
                    Id = evaluation.Id,
                    Name = evaluation.Name,
                    Description = evaluation.Template.Description,
                    Status = evaluation.Status,
                    EndDateTime = evaluation.EndDateTime,
                    CollaboratorName = user.FullName,
                    IsAutoEvaluation = evaluation.Template.IsAutoEvaluation
                }).ToListAsync();

            evaluationsResult.AddRange(await EvaluationRepository
            .GetAll()
            .Include(evaluation => evaluation.Template)
            .Where(evaluation => evaluation.UserId == userId)
            .Where(evaluation => evaluation.Status == EvaluationStatus.Validated)
            .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
            .Select(evaluation => new EvaluationSummaryValueObject
            {
                Term = evaluation.Term,
                Id = evaluation.Id,
                Name = evaluation.Name,
                Description = evaluation.Template.Description,
                Status = evaluation.Status,
                EndDateTime = evaluation.EndDateTime,
                CollaboratorName = user.FullName,
                IsAutoEvaluation = evaluation.Template.IsAutoEvaluation
            }).ToListAsync());

            return evaluationsResult;
        }

        public async Task<ICollection<EvaluationSummaryValueObject>> GetUserOrganizationUnitCollaboratorsEvaluationsHistoryAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);

            List<EvaluationSummaryValueObject> evaluationsSummary = new List<EvaluationSummaryValueObject>();

            List<long> userIds = (await UserManager.GetSubordinatesTree(supervisorUser))
                .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                .Select(user => user.Id)
                .ToList();


            foreach (long idUser in userIds)
            {
                List<EvaluationSummaryValueObject> currentUserEvaluations = (await GetUserEvaluationsHistoryAsync(idUser))
                    .ToList();
                evaluationsSummary.AddRange(currentUserEvaluations);
            }

            return evaluationsSummary;
        }

        public async Task<ICollection<CollaboratorsPendingObjectivesSummaryValueObject>> GetUserOrganizationUnitObjectivesHistoryAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);

            bool isSupervisor = await UserManager.IsInRoleAsync(supervisorUser, StaticRoleNames.Tenants.Supervisor);

            if (!isSupervisor)
            {
                throw new UserFriendlyException($"El usuario {supervisorUser.FullName} no autorizado.");
            }

            List<CollaboratorsPendingObjectivesSummaryValueObject> collaboratorsPendingObjectivesSummary = new List<CollaboratorsPendingObjectivesSummaryValueObject>();

            List<User> users = new List<User>();

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                users = UserManager.Users
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .Distinct()
                    .ToList();
            }

            foreach (User user in users)
            {
                List<EvaluationObjectivesSummaryValueObject> userObjectivesSummary = await GetUserObjectivesHistoryAsync(user.Id);

                collaboratorsPendingObjectivesSummary.Add(new CollaboratorsPendingObjectivesSummaryValueObject
                {
                    CollaboratorFullName = user.FullName,
                    CollaboratorEmployeeNumber = user.EmployeeNumber,
                    ObjectivesSummary = userObjectivesSummary,
                    TotalPendingObjectives = userObjectivesSummary
                        .Where(objective => objective.Status != EvaluationQuestionStatus.Answered)
                        .Count(objective => objective.Status != EvaluationQuestionStatus.Validated),
                    AccomplishedObjectives = userObjectivesSummary
                        .Where(objective => objective.Status == EvaluationQuestionStatus.Answered)
                        .Count(objective => objective.Status == EvaluationQuestionStatus.Validated)
                });
            }


            return collaboratorsPendingObjectivesSummary;
        }

        public async Task<List<EvaluationObjectivesSummaryValueObject>> GetUserObjectivesHistoryAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            List<EvaluationObjectivesSummaryValueObject> evaluationObjectivesSummaryValueObjects = new List<EvaluationObjectivesSummaryValueObject>();

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {
                evaluationObjectivesSummaryValueObjects = await EvaluationQuestionRepository
                .GetAll()
                .Where(evaluationQuestion => !evaluationQuestion.IsDeleted)
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .ThenInclude(evaluation => evaluation.Template)
                .Where(evaluationQuestion => !evaluationQuestion.Evaluation.IsDeleted)
                .Where(evaluationQuestion => !evaluationQuestion.Evaluation.Template.IsDeleted)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.Template.IsAutoEvaluation)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .OfType<NotEvaluableQuestion>()
                .Select(evaluationQuestion => new EvaluationObjectivesSummaryValueObject
                {
                    Status = evaluationQuestion.Status,
                    Name = evaluationQuestion.Text,
                    DeliveryDate = evaluationQuestion.NotEvaluableAnswer.CommitmentTime,
                    Id = evaluationQuestion.Id,
                    Binnacle = evaluationQuestion.Binnacle.Select(objectiveBinnacle => new ObjectiveBinnacleValueObject
                    {
                        Id = objectiveBinnacle.Id,
                        EvaluationQuestionId = objectiveBinnacle.EvaluationQuestionId,
                        Text = objectiveBinnacle.Text,
                        CreationTime = objectiveBinnacle.CreationTime,
                        UserName = UserManager.Users.Single(user => user.Id == objectiveBinnacle.CreatorUserId).FullName
                    }).ToList(),
                    isNotEvaluable = true
                }).ToListAsync();

                evaluationObjectivesSummaryValueObjects.AddRange(await EvaluationQuestionRepository
                .GetAll()
                .Where(evaluationQuestion => !evaluationQuestion.IsDeleted)
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .Where(evaluationQuestion => !evaluationQuestion.Evaluation.IsDeleted)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .Where(evaluationQuestion => evaluationQuestion.Status != EvaluationQuestionStatus.Validated)
                .OfType<EvaluationMeasuredQuestion>()
                .Select(evaluationQuestion => new EvaluationObjectivesSummaryValueObject
                {
                    Status = evaluationQuestion.Status,
                    Name = evaluationQuestion.MeasuredQuestion.Text,
                    Deliverable = evaluationQuestion.MeasuredQuestion.Deliverable,
                    DeliveryDate = evaluationQuestion.TerminationDateTime,
                    Id = evaluationQuestion.Id,
                    Binnacle = evaluationQuestion.Binnacle.Select(objectiveBinnacle => new ObjectiveBinnacleValueObject
                    {
                        Id = objectiveBinnacle.Id,
                        EvaluationQuestionId = objectiveBinnacle.EvaluationQuestionId,
                        Text = objectiveBinnacle.Text,
                        CreationTime = objectiveBinnacle.CreationTime,
                        UserName = UserManager.Users.Single(user => user.Id == objectiveBinnacle.CreatorUserId).FullName
                    }).ToList(),
                    isNotEvaluable = false
                }).ToListAsync());
            }

            return evaluationObjectivesSummaryValueObjects
                .OrderBy(dashboard => dashboard.DeliveryDate)
                .ToList();
        }

        public async Task<List<EvaluationObjectivesSummaryValueObject>> GetUserObjectivesHistory(long? userId = null)
        {
            List<EvaluationObjectivesSummaryValueObject> evaluationObjectivesSummaryValueObjects = await GetUserObjectivesHistoryAsync(userId);
            return evaluationObjectivesSummaryValueObjects
                .OrderBy(dashboard => dashboard.DeliveryDate)
                .ToList();
        }

        public async Task<List<EvaluationActionValueObject>> GetUserActionsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            List<EvaluationActionValueObject> evaluationObjectivesSummaryValueObjects = await EvaluationQuestionRepository
                .GetAll()
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .ThenInclude(evaluation => evaluation.Template)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .OfType<EvaluationUnmeasuredQuestion>()
                .Select(evaluationQuestion => new EvaluationActionValueObject
                {
                    Description = evaluationQuestion.UnmeasuredAnswer.Action,
                    Responsible = evaluationQuestion.UnmeasuredAnswer.Text,
                    DeliveryDate = evaluationQuestion.UnmeasuredAnswer.CommitmentDate
                })
                .Where(evaluationQuestion => evaluationQuestion.Description != "true")
                .Where(evaluationQuestion => evaluationQuestion.Description != "false")
                .Where(evaluationQuestion => evaluationQuestion.Description != "")
                .Where(evaluationQuestion => evaluationQuestion.Description != null)
                .ToListAsync();

            return evaluationObjectivesSummaryValueObjects
                .OrderBy(dashboard => dashboard.DeliveryDate)
                .ToList();
        }

        public async Task<List<EvaluationActionValueObject>> GetUserOrganizationUnitCollaboratorsActionsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);

            List<EvaluationActionValueObject> actionSummaryValueObjects = new List<EvaluationActionValueObject>();

            List<User> users = UserManager.Users
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .ToList();

            foreach (User user in users)
            {
                List<EvaluationActionValueObject> newListActions = await EvaluationQuestionRepository
                .GetAll()
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .ThenInclude(evaluation => evaluation.Template)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == user.Id)
                .OfType<EvaluationUnmeasuredQuestion>()
                .Select(evaluationQuestion => new EvaluationActionValueObject
                {
                    User = user.FullName,
                    Description = evaluationQuestion.UnmeasuredAnswer.Action,
                    Responsible = evaluationQuestion.UnmeasuredAnswer.Text,
                    DeliveryDate = evaluationQuestion.UnmeasuredAnswer.CommitmentDate
                })
                .Where(evaluationQuestion => evaluationQuestion.Description != "true")
                .Where(evaluationQuestion => evaluationQuestion.Description != "false")
                .Where(evaluationQuestion => evaluationQuestion.Description != "")
                .Where(evaluationQuestion => evaluationQuestion.Description != null)
                .ToListAsync();

                actionSummaryValueObjects.AddRange(newListActions);
            }

            return actionSummaryValueObjects
                .OrderBy(dashboard => dashboard.DeliveryDate)
                .ToList();
        }

        public async Task<ICollection<EvaluationSummaryValueObject>> GetBossEvaluationsHistoryAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);

            List<long> userIds = UserManager.Users
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .Select(user => user.Id)
                    .ToList();

            List<EvaluationSummaryValueObject> evaluationsSummary = new List<EvaluationSummaryValueObject>();

            evaluationsSummary.AddRange(
                await EvaluationRepository
                    .GetAll()
                    .Include(evaluation => evaluation.Template)
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Where(evaluation => evaluation.Status != EvaluationStatus.Pending)
                    .Where(evaluation => evaluation.Status != EvaluationStatus.NonInitiated)
                    .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                    .Select(evaluation => new EvaluationSummaryValueObject
                    {
                        Term = evaluation.Term,
                        Id = evaluation.Id,
                        Name = evaluation.Name,
                        Description = evaluation.Template.Description,
                        Status = evaluation.Status,
                        EndDateTime = evaluation.EndDateTime,
                        CollaboratorName = evaluation.User.FullName,
                        IsAutoEvaluation = evaluation.Template.IsAutoEvaluation
                    }).ToListAsync()
            );

            evaluationsSummary.AddRange(
                await EvaluationRepository
                    .GetAll()
                    .Include(evaluation => evaluation.Template)
                    .Where(evaluation => userIds.Distinct().Contains(evaluation.UserId))
                    .Where(evaluation => evaluation.Template.IsAutoEvaluation)
                    .Where(evaluation => !evaluation.IsActive)
                    .Select(evaluation => new EvaluationSummaryValueObject
                    {
                        Term = evaluation.Term,
                        Id = evaluation.Id,
                        Name = evaluation.Name,
                        Description = evaluation.Template.Description,
                        Status = evaluation.Status,
                        EndDateTime = evaluation.EndDateTime,
                        CollaboratorName = evaluation.User.FullName,
                        IsAutoEvaluation = evaluation.Template.IsAutoEvaluation
                    }).ToListAsync()
            );

            return evaluationsSummary;
        }

        public async Task<List<EvaluationObjectivesSummaryValueObject>> GetCollaboratorObjectivesHistoryAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();
            List<long> evaluationIds = new List<long>();

            evaluationIds = await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Template.IsAutoEvaluation)
                .OrderByDescending(evaluation => evaluation.Id)
                .Select(evaluation => evaluation.Id)
                .ToListAsync();

            evaluationIds.Remove(evaluationIds.First());

            List<EvaluationObjectivesSummaryValueObject> evaluationObjectivesSummaryValueObjects = await EvaluationQuestionRepository
                .GetAll()
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .ThenInclude(evaluation => evaluation.Template)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.Template.IsAutoEvaluation)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .Where(evaluationQuestion => evaluationIds.Contains(evaluationQuestion.Evaluation.Id))
                .OrderByDescending(evaluationQuestion => evaluationQuestion.Evaluation.Id)
                .OfType<NotEvaluableQuestion>()
                .Select(evaluationQuestion => new EvaluationObjectivesSummaryValueObject
                {
                    Status = evaluationQuestion.Status,
                    Name = evaluationQuestion.Text,
                    DeliveryDate = evaluationQuestion.NotEvaluableAnswer.CommitmentTime,
                    Id = evaluationQuestion.Id,
                    Binnacle = evaluationQuestion.Binnacle.Select(objectiveBinnacle => new ObjectiveBinnacleValueObject
                    {
                        Id = objectiveBinnacle.Id,
                        EvaluationQuestionId = objectiveBinnacle.EvaluationQuestionId,
                        Text = objectiveBinnacle.Text,
                        CreationTime = objectiveBinnacle.CreationTime,
                        UserName = UserManager.Users.Single(user => user.Id == objectiveBinnacle.CreatorUserId).FullName
                    }).ToList(),
                    isNotEvaluable = true,
                    isNextObjective = evaluationQuestion.Section.ParentSection.Name == "Objetivos" ? true : false
                }).ToListAsync();

            return evaluationObjectivesSummaryValueObjects
                .Where(dashboard => dashboard.isNextObjective)
                .OrderBy(dashboard => dashboard.DeliveryDate)
                .ToList();
        }
    }
}