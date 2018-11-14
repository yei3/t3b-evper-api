namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Domain.Services;
    using System.Threading.Tasks;
    using ValueObjects;
    using Abp.Domain.Entities;
    using Abp.Domain.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Capabilities;
    using Objectives;
    using System.Collections.Generic;

    public class EvaluationManager : DomainService, IEvaluationManager
    {
        private readonly IRepository<Objective, long> ObjectiveRepository;
        private readonly IRepository<Capability, long> CapabilityRepository;
        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<EvaluationUser, long> EvaluationUserRepository;

        public EvaluationManager(IRepository<Objective, long> objectiveRepository, IRepository<Capability, long> capabilityRepository, IRepository<Evaluation, long> evaluationRepository, IRepository<EvaluationUser, long> evaluationUserRepository)
        {
            ObjectiveRepository = objectiveRepository;
            CapabilityRepository = capabilityRepository;
            EvaluationRepository = evaluationRepository;
            EvaluationUserRepository = evaluationUserRepository;
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

        public async Task<long> AddEvaluationCapabilityAndGetIdAsync(AddEvaluationCapabilityValueObject addEvaluationCapabilityValueObject)
        {
            try
            {
                Capability capability = new Capability(
                    addEvaluationCapabilityValueObject.EvaluationId,
                    addEvaluationCapabilityValueObject.Index,
                    addEvaluationCapabilityValueObject.Description,
                    true,
                    addEvaluationCapabilityValueObject.Name
                    );

                return await CapabilityRepository.InsertAndGetIdAsync(capability);
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