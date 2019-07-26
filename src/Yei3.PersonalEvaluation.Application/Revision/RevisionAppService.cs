using System;
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
using Yei3.PersonalEvaluation.Evaluations.Sections;
using Yei3.PersonalEvaluation.Evaluations.Questions;

namespace Yei3.PersonalEvaluation.Revision
{
    public class RevisionAppService : ApplicationService, IRevisionAppService
    {
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<Evaluations.Sections.Section, long> SectionRepository;
        private readonly IRepository<MeasuredQuestion, long> MeasuredQuestionRepository;
        private readonly IRepository<EvaluationMeasuredQuestion, long> _evaluationQuestionRepository;

        public RevisionAppService(IRepository<Evaluation, long> evaluationRepository, IRepository<Evaluations.Sections.Section, long> sectionRepository, IRepository<MeasuredQuestion, long> measuredQuestionRepository, IRepository<EvaluationMeasuredQuestion, long> evaluationQuestionRepository)
        {
            EvaluationRepository = evaluationRepository;
            SectionRepository = sectionRepository;
            MeasuredQuestionRepository = measuredQuestionRepository;
            _evaluationQuestionRepository = evaluationQuestionRepository;
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

            if (evaluation.Template.IncludePastObjectives)
            {
                Evaluations.Sections.Section evaNextObjectivesSection = evaluation
                    .Template
                    .Sections
                    .Single(section => section.Name == AppConsts.SectionNextObjectivesName);

                Evaluations.Sections.Section autoNextObjectivesSection = autoEvaluation
                    .Template
                    .Sections
                    .Single(section => section.Name == AppConsts.SectionNextObjectivesName);

                Evaluations.Sections.Section evaNextObjectivesChildSection = evaNextObjectivesSection.ChildSections.Single();
                
                Evaluations.Sections.Section autoNextObjectivesChildSection = autoNextObjectivesSection.ChildSections.Single();

                if (autoNextObjectivesChildSection.IsNullOrDeleted())
                {
                    // throw new UserFriendlyException($"No hay objetivos para clonar, revise la auto evalución {autoEvaluation.Id}.");
                }
                // TODO: Clonar Objetivos, aqu[i] esta la magia
                var evaNotEvaluableQuestions = evaNextObjectivesChildSection?.NotEvaluableQuestions
                    .Where(question => question.EvaluationId == autoEvaluation.Id)
                    .ToList();

                var autoNotEvaluableQuestions = autoNextObjectivesChildSection?.NotEvaluableQuestions
                    .Where(question => question.EvaluationId == autoEvaluation.Id)
                    .ToList();

                foreach (var notEvaluableQuestion in autoNotEvaluableQuestions)
                {
                    // var newMeasuredQuestion = MeasuredQuestionRepository.Insert(
                    //     new MeasuredQuestion(
                    //         measuredQuestion.Text,
                    //         measuredQuestion.QuestionType,
                    //         measuredQuestion.IsQualifiable,
                    //         measuredQuestion.Expected,
                    //         measuredQuestion.ExpectedText,
                    //         measuredQuestion.Relation,
                    //         measuredQuestion.Deliverable
                    //     )
                    // );

                    // newMeasuredQuestion.SectionId = nextObjectivesChildSection.Id;
                }

                foreach (var notEvaluableQuestion in evaNotEvaluableQuestions)
                {
                    _evaluationQuestionRepository.Delete(notEvaluableQuestion);
                }

                    // CurrentUnitOfWork.SaveChanges();
            }

            evaluation.ValidateEvaluation();

            return Task.CompletedTask;
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

            evaluation.ScheduleReview();
            evaluation.Revision.SetRevisionTime(input.RevisionTime);
        }
    }
}