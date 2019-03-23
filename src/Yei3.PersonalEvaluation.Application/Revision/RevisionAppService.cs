﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Section;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Revision.Dto;

namespace Yei3.PersonalEvaluation.Revision
{
    public class RevisionAppService : ApplicationService, IRevisionAppService
    {
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<Evaluations.Sections.Section, long> SectionRepository;

        public RevisionAppService(IRepository<Evaluation, long> evaluationRepository, IRepository<Evaluations.Sections.Section, long> sectionRepository)
        {
            EvaluationRepository = evaluationRepository;
            SectionRepository = sectionRepository;
        }

        public Task ReviseEvaluation(long evaluationId)
        {
            Evaluation evaluation = EvaluationRepository
               .GetAll()
               .Include(evaluations => evaluations.Template)
               .ThenInclude(template => template.Sections)
               .ThenInclude(sections => sections.ChildSections)
               .FirstOrDefault(evaluations => evaluations.Id == evaluationId);

            if (evaluation.IsNullOrDeleted())
            {
               return Task.CompletedTask;
            }

            Evaluation autoEvaluation = EvaluationRepository
                .GetAll()
                .Include(evaluations => evaluations.User)
                .Include(evaluations => evaluations.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationMeasuredQuestion)evaluationQuestion).MeasuredAnswer)
                .Include(evaluations => evaluations.Questions)
                .ThenInclude(evaluationQuestion => ((EvaluationUnmeasuredQuestion)evaluationQuestion).UnmeasuredAnswer)
                .Include(evaluations => evaluations.Questions)
                .ThenInclude(evaluationQuestion => ((Evaluations.EvaluationQuestions.NotEvaluableQuestion)evaluationQuestion).NotEvaluableAnswer)
                .Include(evaluations => evaluations.Template)
                .ThenInclude(evaluationTemplate => evaluationTemplate.Sections)
                .Include(evaluations => evaluations.Template.Sections)
                .ThenInclude(section => section.ChildSections)
                .Include(evaluations => evaluations.Template.Sections)
                .ThenInclude(section => section.MeasuredQuestions)
                .Include(evaluations => evaluations.Template.Sections)
                .ThenInclude(section => section.UnmeasuredQuestions)
                .Include(evaluations => evaluations.Template.Sections)
                .ThenInclude(section => section.NotEvaluableQuestions)
                .Where(evaluations => evaluations.Term == evaluation.Term)
                .Where(evaluations => evaluations.UserId == evaluation.UserId)
                .OrderByDescending(evaluations => evaluations.CreationTime)
                .FirstOrDefault(evaluations => evaluations.Id != evaluation.Id);

            if (autoEvaluation.IsNullOrDeleted())
            {
                return Task.CompletedTask;
            }

            Evaluations.Sections.Section nextObjectivesSection = autoEvaluation
                .Template
                .Sections
                .Single(section => section.Name == AppConsts.SectionNextObjectivesName);

            Evaluations.Sections.Section currentSection = evaluation
                .Template
                .Sections
                .Single(section => section.Name == AppConsts.SectionNextObjectivesName);

            foreach (Evaluations.Sections.Section currentSectionChildSection in currentSection.ChildSections)
            {
                SectionRepository.Delete(currentSectionChildSection.Id);
            }

            SectionRepository.Delete(currentSection.Id);

            var importedSection = nextObjectivesSection.NoTracking(autoEvaluation.EvaluationId, autoEvaluation.Id, evaluation.EvaluationId, evaluation.Id);

            SectionRepository.Insert(importedSection);

            evaluation.Revision.MarkAsRevised();
            
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