namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Application.Services;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Dto;
    using System.Collections.Generic;

    public interface IEvaluationAppService : IAsyncCrudAppService<EvaluationDto, long, GetAllEvaluationsInputDto, CreateEvaluationDto>
    {
        Task<ICollection<EntityDto<long>>> EvaluateUsersAndGetIdsAsync(EvaluateUsersInputDto evaluateUsersInputDto);
    }
}