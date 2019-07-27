using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Section;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Revision.Dto;
using Yei3.PersonalEvaluation.Evaluations.Sections;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Revision
{
    public class RevisionAppService : ApplicationService, IRevisionAppService
    {
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<Evaluations.Sections.Section, long> SectionRepository;
        private readonly IRepository<MeasuredQuestion, long> MeasuredQuestionRepository;
        private readonly IRepository<NotEvaluableAnswer, long> _notEvaluableAnswerRepository;
        private readonly IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> _evaluationQuestionRepository;

        public RevisionAppService(
            IRepository<Evaluation, long> evaluationRepository,
            IRepository<Evaluations.Sections.Section, long> sectionRepository,
            IRepository<MeasuredQuestion, long> measuredQuestionRepository,
            IRepository<NotEvaluableAnswer, long> notEvaluableAnswerRepository,
            IRepository<Evaluations.EvaluationQuestions.NotEvaluableQuestion, long> evaluationQuestionRepository
        )
        {
            EvaluationRepository = evaluationRepository;
            SectionRepository = sectionRepository;
            MeasuredQuestionRepository = measuredQuestionRepository;
            _notEvaluableAnswerRepository = notEvaluableAnswerRepository;
            _evaluationQuestionRepository = evaluationQuestionRepository;
        }

        public async Task ReviseEvaluation(long evaluationId)
        {
            Evaluation evaluation = EvaluationRepository
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
                .FirstOrDefault(evaluations => evaluations.Id == evaluationId);

            if (evaluation.IsNullOrDeleted())
            {
                // return Task.CompletedTask;
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
                // return Task.CompletedTask;
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

                Evaluations.Sections.Section evaObjectivesSection = evaNextObjectivesSection.ChildSections.Single();
                
                Evaluations.Sections.Section autoObjectivesSection = autoNextObjectivesSection.ChildSections.Single();

                if (autoObjectivesSection.IsNullOrDeleted())
                {
                    // throw new UserFriendlyException($"No hay objetivos para clonar, revise la auto evalución {autoEvaluation.Id}.");
                }
                // TODO: Clone Objectives, here is da magic
                var evaNotEvaluableQuestions = evaObjectivesSection?.NotEvaluableQuestions
                    .Where(question => question.EvaluationId == evaluationId);

                var autoNotEvaluableQuestions = autoObjectivesSection?.NotEvaluableQuestions
                    .Where(question => question.EvaluationId == autoEvaluation.Id);

                foreach (var autoNotEvaluableQuestion in autoNotEvaluableQuestions)
                {
                    Evaluations.EvaluationQuestions.NotEvaluableQuestion currentQuestion =
                        new Evaluations.EvaluationQuestions.NotEvaluableQuestion(
                            evaObjectivesSection.Id,
                            autoNotEvaluableQuestion.Text,
                            evaluationId,
                            autoNotEvaluableQuestion.NotEvaluableAnswer.CommitmentTime,
                            autoNotEvaluableQuestion.Status
                        ){
                            SectionId = evaObjectivesSection.Id
                        };

                    currentQuestion.SetAnswer(
                        evaluationId,
                        autoNotEvaluableQuestion.NotEvaluableAnswer.Text,
                        autoNotEvaluableQuestion.NotEvaluableAnswer.CommitmentTime
                    );

                    await _evaluationQuestionRepository.InsertAsync(currentQuestion);
                }
                // TODO: Remove current Objectives
                foreach (var evaNotEvaluableQuestion in evaNotEvaluableQuestions)
                {
                    NotEvaluableAnswer notEvaluableAnswer = _notEvaluableAnswerRepository
                        .FirstOrDefault(answer => answer.EvaluationQuestionId == evaNotEvaluableQuestion.Id);
                    await _notEvaluableAnswerRepository.DeleteAsync(notEvaluableAnswer);
                    await _evaluationQuestionRepository.DeleteAsync(evaNotEvaluableQuestion);
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }

            evaluation.ValidateEvaluation();
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