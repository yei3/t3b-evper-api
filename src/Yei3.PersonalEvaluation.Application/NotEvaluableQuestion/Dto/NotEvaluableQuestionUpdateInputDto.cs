using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations;

namespace Yei3.PersonalEvaluation.NotEvaluableQuestion.Dto
{
    [AutoMap(typeof(Evaluations.EvaluationQuestions.NotEvaluableQuestion))]
    public class NotEvaluableQuestionUpdateInputDto : EntityDto<long>
    {
        public long SectionId { get; set; }
        public long EvaluationId { get; set; }
        public string Text { get; set; }
        public EvaluationStatus Status { get; set; }
    }
}