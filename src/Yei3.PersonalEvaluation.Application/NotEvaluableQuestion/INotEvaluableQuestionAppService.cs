using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;
using Yei3.PersonalEvaluation.NotEvaluableQuestion.Dto;
using Yei3.PersonalEvaluation.Question.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableQuestion
{
    public interface INotEvaluableQuestionAppService : IAsyncCrudAppService<NotEvaluableQuestionDto, long, QuestionGetAllInputDto, NotEvaluableQuestionDto, NotEvaluableQuestionUpdateInputDto>
    {
        Task<List<EvaluationObjectivesSummaryValueObject>> GetSummary(long evaluationId);
        Task UpdateStatus(UpdateStatusInputDto input);
    }
}