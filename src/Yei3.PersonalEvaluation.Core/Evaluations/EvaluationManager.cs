using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
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
                .Where(evaluation => evaluation.EndDateTime > DateTime.Now)
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
                //.Where(evaluation => evaluation.Template.IsAutoEvaluation) !!!!!! luis angel me mando el dia de la entrega. !!!!!!
                .Where(evaluation => evaluation.EndDateTime > DateTime.Now)
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
                .Where(evaluation => evaluation.Status == EvaluationStatus.Finished)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Include(evaluation => evaluation.Template)
                .Include(evaluation => evaluation.Revision)
                .Select(evaluation => new RevisionSummaryValueObject
                {
                    EvaluationId = evaluation.Id,
                    Term = evaluation.Term,
                    Status = evaluation.Revision.Status,
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

            List<EvaluationObjectivesSummaryValueObject> evaluationObjectivesSummaryValueObjects = await EvaluationQuestionRepository
                .GetAll()
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .Where(evaluationQuestion => evaluationQuestion.Status != EvaluationQuestionStatus.Validated)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.EndDateTime > DateTime.Now)
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
                .Include(evaluationQuestion => evaluationQuestion.Evaluation)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .Where(evaluationQuestion => evaluationQuestion.Status != EvaluationQuestionStatus.Validated)
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.EndDateTime > DateTime.Now)
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

        public async Task<int> GetUserPendingEvaluationsCountAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.NonInitiated)
                .Where(evaluation => !evaluation.Template.IsAutoEvaluation)
                .Where(evaluation => evaluation.EndDateTime > DateTime.Now)
                .CountAsync();
        }

        public Task<int> GetUserPendingObjectivesCountAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();
            int pendingObjectivesCountAsync = 0;

            IQueryable<Evaluation> pendingEvaluations = EvaluationRepository
                .GetAll()
                .Include(evaluation => evaluation.Questions)
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.NonInitiated || evaluation.Status == EvaluationStatus.Pending)
                .Where(evaluation => evaluation.EndDateTime > DateTime.Now);

            foreach (Evaluation pendingEvaluation in pendingEvaluations)
            {
                pendingObjectivesCountAsync += pendingEvaluation
                    .Questions
                    .OfType<EvaluationMeasuredQuestion>()
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
            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            List<EvaluationSummaryValueObject> evaluationsSummary = new List<EvaluationSummaryValueObject>();

            foreach (Abp.Organizations.OrganizationUnit organizationUnit in organizationUnits)
            {
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit, true))
                    .Distinct()
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .ToList();

                foreach (User user in users)
                {
                    List<EvaluationSummaryValueObject> currentUserEvaluations = (await GetUserPendingEvaluationsAsync(user.Id))
                        .ToList();
                    evaluationsSummary.AddRange(currentUserEvaluations);
                }
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

            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            List<RevisionSummaryValueObject> revisionsSummary = new List<RevisionSummaryValueObject>();

            foreach (Abp.Organizations.OrganizationUnit organizationUnit in organizationUnits)
            {
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit, true))
                    .Distinct()
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .ToList();

                foreach (User user in users)
                {
                    List<RevisionSummaryValueObject> currentUserPendingRevisions = (await GetUserPendingEvaluationRevisionsAsync(user.Id))
                        .ToList();

                    revisionsSummary.AddRange(currentUserPendingRevisions);
                }
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

            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            List<CollaboratorsPendingObjectivesSummaryValueObject> collaboratorsPendingObjectivesSummary = new List<CollaboratorsPendingObjectivesSummaryValueObject>();

            foreach (Abp.Organizations.OrganizationUnit organizationUnit in organizationUnits)
            {
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit, true))
                    .Distinct()
                    .Where(user => user.ImmediateSupervisor == supervisorUser.JobDescription)
                    .ToList();

                foreach (User user in users)
                {
                    List<EvaluationObjectivesSummaryValueObject> userObjectivesSummary = await GetUserPendingObjectiveAsync(user.Id);

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
            }

            return collaboratorsPendingObjectivesSummary;
        }
    }
}