using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Revision.Dto;

namespace Yei3.PersonalEvaluation.Revision
{
    public class RevisionAppService : ApplicationService, IRevisionAppService
    {
        private readonly IRepository<Evaluation, long> EvaluationRepository;

        public RevisionAppService(IRepository<Evaluation, long> evaluationRepository)
        {
            EvaluationRepository = evaluationRepository;
        }

        public Task ReviseEvaluation(long evaluationId)
        {
            Evaluation currentEvaluation = EvaluationRepository
               .GetAll()
               .Include(evaluation => evaluation.Template)
               .ThenInclude(template => template.Sections)
               .ThenInclude(section => section.ChildSections)
               .FirstOrDefault(evaluation => evaluation.Id == evaluationId);

            if (currentEvaluation.IsNullOrDeleted())
            {
                throw new EntityNotFoundException(typeof(Evaluation), evaluationId);
            }

            //Planchar Objetivos

            //if (currentEvaluation.IsNullOrDeleted())
            //{
            //    return Task.CompletedTask;
            //}

            //currentEvaluation.ClosingComment = evaluationClose.Comment;

            //Evaluation pairEvaluation = EvaluationRepository
            //    .GetAll()
            //    .Include(evaluation => evaluation.Template)
            //    .ThenInclude(template => template.Sections)
            //    .ThenInclude(section => section.ChildSections)
            //    .Where(evaluation => evaluation.Term == currentEvaluation.Term)
            //    .Where(evaluation => evaluation.UserId == currentEvaluation.UserId)
            //    .OrderByDescending(evaluation => evaluation.CreationTime)
            //    .FirstOrDefault(evaluation => evaluation.Id != currentEvaluation.Id);

            //if (pairEvaluation.IsNullOrDeleted())
            //{
            //    return Task.CompletedTask;
            //}

            //Sections.Section nextObjectivesSection = pairEvaluation
            //    .Template
            //    .Sections
            //    .Single(section => section.Name == AppConsts.SectionNextObjectivesName);

            //Sections.Section currentSection = currentEvaluation
            //    .Template
            //    .Sections
            //    .Single(section => section.Name == AppConsts.SectionNextObjectivesName);

            ////SectionRepository.Delete(currentSection.Id);

            ////SectionRepository.Insert(nextObjectivesSection.NoTracking(currentEvaluation.Id));
            currentEvaluation.Revision.MarkAsRevised();
            return Task.CompletedTask;
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

        public async Task FinishEvaluation(long evaluationId)
        {
            Evaluation evaluation = await EvaluationRepository
                .GetAll()
                .Include(currentEvaluation => currentEvaluation.Revision)
                .FirstOrDefaultAsync(currentEvaluation => currentEvaluation.Id == evaluationId);

            evaluation.FinishEvaluation();
        }

        public async Task UnfininshEvaluation(long evaluationId)
        {
            Evaluation evaluation = await EvaluationRepository
                .GetAll()
                .Include(currentEvaluation => currentEvaluation.Revision)
                .FirstOrDefaultAsync(currentEvaluation => currentEvaluation.Id == evaluationId);

            evaluation.UnfinishEvaluation();
        }

        public async Task UpdateRevisionDate(UpdateRevisionDateInputDto input)
        {
            Evaluation evaluation = await EvaluationRepository
                .GetAll()
                .Include(currentEvaluation => currentEvaluation.Revision)
                .FirstOrDefaultAsync(currentEvaluation => currentEvaluation.Id == input.EvaluationId);

            evaluation.Revision.SetRevisionTime(input.RevisionTime);
        }
    }
}