using Abp.Collections.Extensions;

namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Domain.Services;
    using System.Threading.Tasks;
    using ValueObjects;
    using Abp.Domain.Entities;
    using Abp.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Objectives;
    using System.Collections.Generic;

    public class EvaluationManager : DomainService, IEvaluationManager
    {
        private readonly IRepository<Objective, long> ObjectiveRepository;
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<EvaluationUser, long> EvaluationUserRepository;
        private readonly IRepository<Section.Section, long> SectionRepository;
        private readonly IRepository<Question.Question, long> QuestionRepository;

        public EvaluationManager(IRepository<Objective, long> objectiveRepository, IRepository<Evaluation, long> evaluationRepository, IRepository<EvaluationUser, long> evaluationUserRepository, IRepository<Section.Section, long> sectionRepository, IRepository<Question.Question, long> questionRepository)
        {
            ObjectiveRepository = objectiveRepository;
            EvaluationRepository = evaluationRepository;
            EvaluationUserRepository = evaluationUserRepository;
            SectionRepository = sectionRepository;
            QuestionRepository = questionRepository;
        }

        public async Task<long> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveValueObject addEvaluationObjectiveValueObject)
        {
            try
            {
                Objective objective = new Objective(
                    addEvaluationObjectiveValueObject.Index,
                    addEvaluationObjectiveValueObject.Description,
                    addEvaluationObjectiveValueObject.EvaluationId,
                    true,
                    addEvaluationObjectiveValueObject.DefinitionOfDone,
                    addEvaluationObjectiveValueObject.DeliveryDate
                );

                return await ObjectiveRepository.InsertAndGetIdAsync(objective);
            }
            catch (DbUpdateException)
            {
                throw new EntityNotFoundException("EvaluationNotFound"); // most certainly the evaluation does not exist
            }
        }

        public async Task<long> AddEvaluationSectionAndGetIdAsync(SectionValueObject addEvaluationSectionValueObject)
        {
            try
            {
                SectionRepository.Delete(section => section.EvaluationId == addEvaluationSectionValueObject.EvaluationId);

                Section.Section rootSection = new Section.Section(
                    addEvaluationSectionValueObject.Name,
                    addEvaluationSectionValueObject.ShowName,
                    addEvaluationSectionValueObject.EvaluationId,
                    null,
                    true);

                long rootSectionId = await SectionRepository.InsertAndGetIdAsync(rootSection);

                foreach (QuestionValueObject questionValueObject in addEvaluationSectionValueObject.Questions)
                {
                    Question.Question question = new Question.Question(
                        questionValueObject.Text,
                        questionValueObject.QuestionType,
                        rootSectionId);

                    await QuestionRepository.InsertAsync(question);
                }

                foreach (SectionValueObject sectionValueObject in addEvaluationSectionValueObject.SubSections)
                {
                    Section.Section currentSection = new Section.Section(
                        sectionValueObject.Name,
                        sectionValueObject.ShowName,
                        sectionValueObject.EvaluationId,
                        rootSectionId,
                        true);

                    long currentSectionId = await SectionRepository.InsertAndGetIdAsync(currentSection);

                    foreach (QuestionValueObject questionValueObject in sectionValueObject.Questions)
                    {
                        Question.Question question = new Question.Question(
                            questionValueObject.Text,
                            questionValueObject.QuestionType,
                            currentSectionId);

                        await QuestionRepository.InsertAsync(question);
                    }
                }

                return rootSectionId;
            }
            catch (DbUpdateException)
            {
                throw new EntityNotFoundException("EvaluationNotFound"); // most certainly the evaluation does not exist
            }
        }

        public async Task<long> AddEvaluationInstructionsAndGetIdAsync(AddEvaluationInstructionsValueObject addEvaluationInstructionsValueObject)
        {
            Evaluation evaluation = await EvaluationRepository
                .FirstOrDefaultAsync(addEvaluationInstructionsValueObject.Id);

            if (evaluation.IsNullOrDeleted())
            {
                throw new EntityNotFoundException($"Evaluacion {addEvaluationInstructionsValueObject.Id} no encontrada");
            }

            evaluation.Instructions = addEvaluationInstructionsValueObject.Instructions;
            await EvaluationRepository.UpdateAsync(evaluation);

            return evaluation.Id;
        }

        public async Task<ICollection<long>> EvaluateUsers(long evaluationId, ICollection<long> userIds)
        {
            Evaluation evaluation = await EvaluationRepository
                .FirstOrDefaultAsync(evaluationId);

            if (evaluation.IsNullOrDeleted())
            {
                throw new EntityNotFoundException($"Evaluacion {evaluationId} no encontrada");
            }

            List<long> evaluationUserIds = new List<long>();

            try
            {
                foreach (long userId in userIds)
                {
                    evaluationUserIds.Add(
                        await EvaluationUserRepository.InsertAndGetIdAsync(new EvaluationUser(evaluationId, userId)));
                }
            }
            catch (DbUpdateException)
            {
                throw new EntityNotFoundException($"Uno de los usuarios \"{evaluationUserIds.ToString()}\" es no valido");  // most certainly the user does not exist
            }

            return evaluationUserIds;
        }
    }
}