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

    public class EvaluationManager : DomainService, IEvaluationManager
    {

        private readonly IRepository<Evaluation, long> EvaluationRepository;
        private readonly IRepository<Objective, long> ObjectiveRepository;
        private readonly IRepository<Capability, long> CapabilityRepository;

        public EvaluationManager(IRepository<Evaluation, long> evaluationRepository, IRepository<Objective, long> objectiveRepository, IRepository<Capability, long> capabilityRepository)
        {
            EvaluationRepository = evaluationRepository;
            ObjectiveRepository = objectiveRepository;
            CapabilityRepository = capabilityRepository;
        }

        public async Task<long> CreateEvaluationAndGetIdAsync(NewEvaluationValueObject newEvaluationValueObject)
        {
            Evaluation evaluation = new Evaluation(newEvaluationValueObject.Term,
                newEvaluationValueObject.EvaluatedUserId, newEvaluationValueObject.EvaluatorUserId);
            return await EvaluationRepository.InsertAndGetIdAsync(evaluation);
        }

        public async Task<long> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveValueObject addEvaluationObjectiveValueObject)
        {
            try
            {
                Objective objective = new Objective(
                    addEvaluationObjectiveValueObject.Index,
                    addEvaluationObjectiveValueObject.Description,
                    addEvaluationObjectiveValueObject.EvaluationId,
                    true
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
    }
}