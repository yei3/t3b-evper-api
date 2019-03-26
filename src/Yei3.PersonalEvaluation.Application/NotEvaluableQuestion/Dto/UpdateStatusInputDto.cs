using Abp.Application.Services.Dto;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.NotEvaluableQuestion.Dto
{
    public class UpdateStatusInputDto : EntityDto<long>
    {
        public EvaluationQuestionStatus Status { get; set; }
    }
}