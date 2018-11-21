using System;
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

        public async Task<long> InsertOrUpdateSectionAndGetIdAsync(SectionValueObject addEvaluationSectionValueObject)
        {
                Section.Section rootSection = new Section.Section(
                    addEvaluationSectionValueObject.Name,
                    addEvaluationSectionValueObject.EvaluationId,
                    true,
                    addEvaluationSectionValueObject.Id);

                return await SectionRepository.InsertOrUpdateAndGetIdAsync(rootSection);
        }

        public async Task<long> InsertOrUpdateSubsectionAndGetIdAsync(SubsectionValueObject addSubsectionValueObject)
        {
            Section.Section rootSection = null;
            try
            {
                rootSection =
                    await SectionRepository.SingleAsync(section => section.Id == addSubsectionValueObject.ParentId);
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Seccion {addSubsectionValueObject.ParentId} no encontrada");
            }

            Section.Section subSection = new Section.Section(
                addSubsectionValueObject.Name,
                rootSection.EvaluationId,
                addSubsectionValueObject.ParentId,
                true,
                addSubsectionValueObject.Id);

            return await SectionRepository.InsertOrUpdateAndGetIdAsync(subSection);
        }

        public async Task<long> InsertOrUpdateQuestionAndGetIdAsync(QuestionValueObject questionValueObject)
        {
            Question.Question question = new Question.Question(
                questionValueObject.Text,
                questionValueObject.QuestionType,
                questionValueObject.SectionId,
                questionValueObject.Id);

            return await QuestionRepository.InsertOrUpdateAndGetIdAsync(question);
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