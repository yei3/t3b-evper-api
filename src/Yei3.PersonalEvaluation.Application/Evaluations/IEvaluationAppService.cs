namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Application.Services;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Dto;
    using System.Collections.Generic;

    public interface IEvaluationAppService : IAsyncCrudAppService<EvaluationDto, long, GetAllEvaluationsInputDto, CreateEvaluationDto>
    {
        Task<EntityDto<long>> AddEvaluationObjectiveAndGetIdAsync(AddEvaluationObjectiveDto addEvaluationObjectiveDto);
        Task<EntityDto<long>> InsertOrUpdateSectionAndGetIdAsync(SectionDto sectionDto);
        Task<EntityDto<long>> InsertOrUpdateSubsectionAndGetIdAsync(SectionDto sectionDto);
        Task<EntityDto<long>> AddEvaluationInstructionsAndGetIdAsync(SetEvaluationInstructionsDto evaluationInstructionsDto);
        Task<EntityDto<long>> InsertOrUpdateQuestionAndGetIdAsync(QuestionDto questionDto);

        Task<ICollection<EntityDto<long>>> EvaluateUsersAndGetIdsAsync(EvaluateUsersInputDto evaluateUsersInputDto);
    }
}