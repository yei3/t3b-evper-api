namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Domain.Services;
    using System.Threading.Tasks;
    using ValueObjects;
    using Abp.Domain.Entities;
    using Abp.Domain.Repositories;
    using Abp.UI;
    using Capabilities;
    using Objectives;

    public class EvaluationManager : DomainService, IEvaluationManager
    {

        private readonly IRepository<Evaluation, long> EvaluationRepository;

        public EvaluationManager(IRepository<Evaluation, long> evaluationRepository)
        {
            EvaluationRepository = evaluationRepository;
        }

        public async Task<long> CreateEvaluationAndGetIdAsync(NewEvaluationValueObject newEvaluationValueObject)
        {
            Evaluation evaluation = new Evaluation(newEvaluationValueObject.Term,
                newEvaluationValueObject.EvaluatedUserId, newEvaluationValueObject.EvaluatorUserId);
            return await EvaluationRepository.InsertAndGetIdAsync(evaluation);
        }

        public async Task<long> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveValueObject addEvaluationObjectiveValueObject)
        {
            Evaluation evaluation = await EvaluationRepository.FirstOrDefaultAsync(addEvaluationObjectiveValueObject.EvaluationId);

            if (evaluation.IsNullOrDeleted())
            {
                throw new UserFriendlyException(404, L("EvaluationNotFound"));
            }

            evaluation.Objectives.Add(new Objective(
                addEvaluationObjectiveValueObject.Index,
                addEvaluationObjectiveValueObject.Description,
                addEvaluationObjectiveValueObject.EvaluationId,
                true
                ));

            return await EvaluationRepository.InsertOrUpdateAndGetIdAsync(evaluation);
        }

        public async Task<long> AddEvaluationCapabilityAndGetIdAsync(AddEvaluationCapabilityValueObject addEvaluationCapabilityValueObject)
        {
            Evaluation evaluation = await EvaluationRepository.FirstOrDefaultAsync(addEvaluationCapabilityValueObject.EvaluationId);

            if (evaluation.IsNullOrDeleted())
            {
                throw new UserFriendlyException(404, L("EvaluationNotFound"));
            }

            evaluation.Capabilities.Add(new Capability(
                addEvaluationCapabilityValueObject.EvaluationId,
                addEvaluationCapabilityValueObject.Index,
                addEvaluationCapabilityValueObject.Description,
                true,
                addEvaluationCapabilityValueObject.Name
                ));

            return await EvaluationRepository.InsertOrUpdateAndGetIdAsync(evaluation);
        }
    }
}