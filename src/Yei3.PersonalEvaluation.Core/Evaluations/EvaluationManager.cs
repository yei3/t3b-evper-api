using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;
using Yei3.PersonalEvaluation.Evaluations.Terms;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;

namespace Yei3.PersonalEvaluation.Evaluations
{
    
    public class EvaluationManager : DomainService, IEvaluationManager
    {

        private readonly IAbpSession AbpSession;
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<EvaluationRevision, long> EvaluationRevisionRepository;
        private readonly IRepository<EvaluationQuestion, long> EvaluationQuestionRepository;
        private readonly UserManager UserManager;

        public EvaluationManager(IAbpSession abpSession, IRepository<Evaluation, long> evaluationRepository, IRepository<EvaluationRevision, long> evaluationRevisionRepository, IRepository<EvaluationQuestion, long> evaluationQuestionRepository, UserManager userManager)
        {
            AbpSession = abpSession;
            EvaluationRepository = evaluationRepository;
            EvaluationRevisionRepository = evaluationRevisionRepository;
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
                .Where(evaluation => evaluation.CreatorUserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.Pending)
                .CountAsync();
        }

        public async Task<List<EvaluationSummaryValueObject>> GetUserPendingAutoEvaluationsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User user = await UserManager.GetUserByIdAsync(userId.Value);

            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.CreatorUserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.Pending)
                .Select(evaluation => new EvaluationSummaryValueObject
                {
                    Term = evaluation.Term,
                    Id = evaluation.Id,
                    Name = evaluation.Template.Name,
                    Status = evaluation.Status,
                    EndDateTime = evaluation.EndDateTime,
                    CollaboratorName = user.FullName
                }).ToListAsync();
        }

        public async Task<List<RevisionSummaryValueObject>> GetUserPendingEvaluationRevisionsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            return await EvaluationRevisionRepository
                .GetAll()
                .Where(revision => revision.Status == EvaluationRevisionStatus.Pending)
                .Where(revision => revision.Evaluation.UserId == userId)
                .Select(revision => new RevisionSummaryValueObject
                {
                    Term = revision.Evaluation.Term,
                    Status = revision.Status,
                    EndDateTime = revision.Evaluation.EndDateTime,
                    Name = revision.Evaluation.Template.Name,
                    RevisionDateTime = revision.RevisionDateTime
                }).ToListAsync();
        }

        public async Task<List<EvaluationObjectivesSummaryValueObject>> GetUserPendingObjectiveAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            return await EvaluationQuestionRepository
                .GetAll()
                .Where(evaluationQuestion => evaluationQuestion.Evaluation.UserId == userId)
                .Where(evaluationQuestion => evaluationQuestion.MeasuredQuestion != null)
                .Select(evaluationQuestion => new EvaluationObjectivesSummaryValueObject
                {
                    Status = evaluationQuestion.Status,
                    Name = evaluationQuestion.MeasuredQuestion.Text,
                    Deliverable = evaluationQuestion.MeasuredQuestion.Deliverable,
                    DeliveryDate = evaluationQuestion.TerminationDateTime
                }).ToListAsync();
        }


        public async Task<int> GetUserPendingEvaluationsCountAsync(long? userId = null)
        {

            userId = userId ?? AbpSession.GetUserId();

            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.Pending)
                .CountAsync();
        }

        public Task<int> GetUserPendingObjectivesCountAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();
            int pendingObjectivesCountAsync = 0;

            IQueryable<Evaluation> pendingEvaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == userId)
                .Where(evaluation => evaluation.Status == EvaluationStatus.Pending);

            foreach (Evaluation pendingEvaluation in pendingEvaluations)
            {
                pendingObjectivesCountAsync += pendingEvaluation.Questions.Count(question => question.Status == EvaluationQuestionStatus.Unanswered);
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
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit))
                    .Distinct()
                    .Where(user =>
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Supervisor).GetAwaiter().GetResult()) &&
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Administrator).GetAwaiter().GetResult()))
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
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit))
                    .Distinct()
                    .Where(user =>
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Supervisor).GetAwaiter().GetResult()) &&
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Administrator).GetAwaiter().GetResult()))
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
                List<User> users = (await UserManager.GetUsersInOrganizationUnit(organizationUnit))
                    .Distinct()
                    .Where(user =>
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Supervisor).GetAwaiter().GetResult()) &&
                        !(UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Administrator).GetAwaiter().GetResult()))
                    .ToList();

                foreach (User user in users)
                {
                    List<EvaluationSummaryValueObject> currentUserEvaluations = await GetUserPendingAutoEvaluationsAsync(user.Id);
                    evaluationsSummary.AddRange(currentUserEvaluations);
                }
            }

            return evaluationsSummary;
        }

        public async Task<ICollection<RevisionSummaryValueObject>> GetUserOrganizationUnitPendingEvaluationRevisionsAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);
            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            List<RevisionSummaryValueObject> revisionsSummary = new List<RevisionSummaryValueObject>();

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
                    List<RevisionSummaryValueObject> currentUserPendingRevisions = await GetUserPendingEvaluationRevisionsAsync(user.Id);
                    revisionsSummary.AddRange(currentUserPendingRevisions);
                }
            }

            return revisionsSummary;
        }

        public async Task<ICollection<CollaboratorsPendingObjectivesSummaryValueObject>> GetUserOrganizationUnitObjectivesSummaryAsync(long? userId = null)
        {
            userId = userId ?? AbpSession.GetUserId();

            User supervisorUser = await UserManager.GetUserByIdAsync(userId.Value);
            List<Abp.Organizations.OrganizationUnit> organizationUnits = await UserManager.GetOrganizationUnitsAsync(supervisorUser);

            List<CollaboratorsPendingObjectivesSummaryValueObject> collaboratorsPendingObjectivesSummary = new List<CollaboratorsPendingObjectivesSummaryValueObject>();

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
                    List<EvaluationObjectivesSummaryValueObject> userObjectivesSummary = await GetUserPendingObjectiveAsync(user.Id);
                    collaboratorsPendingObjectivesSummary.Add(new CollaboratorsPendingObjectivesSummaryValueObject
                    {
                        CollaboratorFullName = user.FullName,
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