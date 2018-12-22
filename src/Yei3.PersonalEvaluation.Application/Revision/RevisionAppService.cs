using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Evaluations;

namespace Yei3.PersonalEvaluation.Revision
{
    public class RevisionAppService : ApplicationService, IRevisionAppService
    {
        private readonly IRepository<Evaluation, long> EvaluationRepository;

        public RevisionAppService(IRepository<Evaluation, long> evaluationRepository)
        {
            EvaluationRepository = evaluationRepository;
        }

        public async Task ReviseEvaluation(long evaluationId)
        {
            Evaluation evaluation = await EvaluationRepository
                .GetAll()
                .Include(currentEvaluation => currentEvaluation.Revision)
                .FirstOrDefaultAsync(currentEvaluation => currentEvaluation.Id == evaluationId);

            if (evaluation.IsNullOrDeleted())
            {
                throw new EntityNotFoundException(typeof(Evaluation), evaluationId);
            }

            evaluation.Revision.MarkAsRevised();
        }

        public async Task UnreviseEvaluation(long evaluationId)
        {
            Evaluation evaluation = await EvaluationRepository
                .GetAll()
                .Include(currentEvaluation => currentEvaluation.Revision)
                .FirstOrDefaultAsync(currentEvaluation => currentEvaluation.Id == evaluationId);

            if (evaluation.IsNullOrDeleted())
            {
                throw new EntityNotFoundException(typeof(Evaluation), evaluationId);
            }

            evaluation.Revision.MarkAsPending();
        }
    }
}