using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yei3.PersonalEvaluation.Evaluations
{
    using Dto;

    public interface IEvaluationAppService
    {
        Task ApplyEvaluationTemplate(CreateEvaluationDto input);
        Task<ICollection<Evaluation>> GetAllAsync();
    }
}