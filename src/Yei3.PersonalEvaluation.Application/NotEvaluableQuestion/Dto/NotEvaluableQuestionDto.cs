using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.Dto;

namespace Yei3.PersonalEvaluation.NotEvaluableQuestion.Dto
{
    [AutoMap(typeof(Evaluations.EvaluationQuestions.NotEvaluableQuestion))]
    public class NotEvaluableQuestionDto : EntityDto<long>
    {
        public long SectionId { get; set; }
        public long EvaluationId { get; set; }
        public string Text { get; set; }
        public EvaluationAnswerDto NotEvaluableAnswer { get; set; }
    }
}