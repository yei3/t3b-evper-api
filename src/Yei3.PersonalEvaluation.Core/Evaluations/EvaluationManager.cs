using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
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

        public EvaluationManager(IAbpSession abpSession, IRepository<Evaluation, long> evaluationRepository, IRepository<EvaluationRevision, long> evaluationRevisionRepository)
        {
            AbpSession = abpSession;
            EvaluationRepository = evaluationRepository;
            EvaluationRevisionRepository = evaluationRevisionRepository;
        }

        public async Task<EvaluationTerm> GetUserNextEvaluationTermAsync()
        {
            var nextEvaluation = await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.Status == EvaluationStatus.NonInitiated)
                .OrderBy(evaluation => evaluation.CreationTime)
                .FirstOrDefaultAsync(evaluation => evaluation.UserId == AbpSession.GetUserId());

            return nextEvaluation == null ? EvaluationTerm.NoTerm : nextEvaluation.Term;
        }

        public async Task<ToDoesSummaryValueObject> GetUserToDoesSummary()
        {
            return new ToDoesSummaryValueObject
            {
                Evaluations = await GetUserPendingEvaluationsCountAsync(),
                AutoEvaluations = await GetUserPendingAutoEvaluationsCountAsync(),
                Objectives = await GetUserPendingObjectivesCountAsync()
            };
        }

        public async Task<int> GetUserPendingAutoEvaluationsCountAsync()
        {
            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == AbpSession.GetUserId())
                .Where(evaluation => evaluation.CreatorUserId == AbpSession.GetUserId())
                .Where(evaluation => evaluation.Status == EvaluationStatus.Pending)
                .CountAsync();
        }

        public async Task<List<EvaluationSummaryValueObject>> GetUserPendingAutoEvaluationsAsync()
        {
            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == AbpSession.GetUserId())
                .Where(evaluation => evaluation.CreatorUserId == AbpSession.GetUserId())
                .Where(evaluation => evaluation.Status == EvaluationStatus.Pending)
                .Select(evaluation => new EvaluationSummaryValueObject
                {
                    Term = evaluation.Term,
                    Id = evaluation.Id,
                    Name = evaluation.Template.Name,
                    Status = evaluation.Status,
                    EndDateTime = evaluation.EndDateTime
                }).ToListAsync();
        }

        public async Task<List<RevisionSummaryValueObject>> GetUserPendingEvaluationRevisionsAsync()
        {
            return await EvaluationRevisionRepository
                .GetAll()
                .Where(revision => revision.Status == EvaluationRevisionStatus.Pending)
                .Where(revision => revision.Evaluation.UserId == AbpSession.GetUserId())
                .Select(revision => new RevisionSummaryValueObject
                {
                    Term = revision.Evaluation.Term,
                    Status = revision.Status,
                    EndDateTime = revision.Evaluation.EndDateTime,
                    Name = revision.Evaluation.Template.Name,
                    RevisionDateTime = revision.RevisionDateTime
                }).ToListAsync();
        }


        public async Task<int> GetUserPendingEvaluationsCountAsync()
        {
            return await EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == AbpSession.GetUserId())
                .Where(evaluation => evaluation.Status == EvaluationStatus.Pending)
                .CountAsync();
        }

        public Task<int> GetUserPendingObjectivesCountAsync()
        {
            int pendingObjectivesCountAsync = 0;

            IQueryable<Evaluation> pendingEvaluations = EvaluationRepository
                .GetAll()
                .Where(evaluation => evaluation.UserId == AbpSession.GetUserId())
                .Where(evaluation => evaluation.Status == EvaluationStatus.Pending);

            foreach (Evaluation pendingEvaluation in pendingEvaluations)
            {
                pendingObjectivesCountAsync += pendingEvaluation.Questions.Count(question => question.Status == EvaluationQuestionStatus.Unanswered);
            }

            return Task.FromResult(pendingObjectivesCountAsync);
        }
    }
}