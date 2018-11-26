using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Yei3.PersonalEvaluation.Evaluations.Dto;

namespace Yei3.PersonalEvaluation.Evaluations
{

    public class EvaluationAppService : AsyncCrudAppService<Evaluation, EvaluationDto, long, GetAllEvaluationsInputDto, CreateEvaluationDto>, IEvaluationAppService
    {
        private readonly IEvaluationManager EvaluationManager;

        public EvaluationAppService(IRepository<Evaluation, long> repository) : base(repository)
        {
        }

        public Task<ICollection<EntityDto<long>>> EvaluateUsersAndGetIdsAsync(EvaluateUsersInputDto evaluateUsersInputDto)
        {
            throw new System.NotImplementedException();
        }
    }
}