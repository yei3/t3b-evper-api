using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;

namespace Yei3.PersonalEvaluation.EvaluationAnswer.Dto
{
    [AutoMap(typeof(UnmeasuredAnswer))]
    public class UnmeasuredAnswerDto : EntityDto<long>
    {
        public string Text { get; set; }
        public long EvaluationQuestionId { get; set; }
        public bool IsActive { get; set; }
    }
}