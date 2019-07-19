using Abp.Application.Services;
using Yei3.PersonalEvaluation.EvaluationObjectives.Dto;
using System.Threading.Tasks;
using Yei3.PersonalEvaluation.Application.EvaluationObjectives.Dto;

namespace Yei3.PersonalEvaluation.EvaluationObjectives
{
    public interface IEvaluationObjectivesAppService : IAsyncCrudAppService<EvaluationObjectiveDto, long, GetAllEvaluationObjectivesDto>
    {
        Task UpdateExpectedValues(UpdateExpectedValuesDto expectedValues);
    }
}