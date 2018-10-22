using System.Threading.Tasks;
using Yei3.PersonalEvaluation.Evaluations.ValueObjects;

namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Domain.Services;

    public class EvaluationManager : DomainService, IEvaluationManager
    {
        public Task CreateEvaluation(NewEvaluationValueObject newEvaluationValueObject)
        {
            throw new System.NotImplementedException();
        }

        public Task AddEvaluationObjective(AddEvaluationObjectiveValueObject addEvaluationObjectiveValueObject)
        {
            throw new System.NotImplementedException();
        }
    }
}