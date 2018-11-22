namespace Yei3.PersonalEvaluation.Evaluations
{
    using Abp.Application.Services;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Dto;
    using System.Collections.Generic;

    public interface IEvaluationAppService : IAsyncCrudAppService<EvaluationDto, long, GetAllEvaluationsInputDto, CreateEvaluationDto>
    {
        Task<EntityDto<long>> InsertOrUpdateSectionAndGetIdAsync(SectionDto sectionDto);
        Task<EntityDto<long>> InsertOrUpdateSubsectionAndGetIdAsync(SectionDto sectionDto);
        Task<EntityDto<long>> InsertOrUpdateQuestionAndGetIdAsync(QuestionDto questionDto);

        Task RemoveEvaluationSection(long sectionId);
        Task RemoveEvaluationQuestion(long questionId);

        Task<ICollection<EntityDto<long>>> EvaluateUsersAndGetIdsAsync(EvaluateUsersInputDto evaluateUsersInputDto);
    }
}