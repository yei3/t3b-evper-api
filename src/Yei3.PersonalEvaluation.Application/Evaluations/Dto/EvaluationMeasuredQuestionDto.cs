using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    [AutoMap(typeof(EvaluationMeasuredQuestion))]
    public class EvaluationMeasuredQuestionDto : EntityDto<long>
    {
        public decimal? Expected { get; set; }
        public string ExpectedText { get; set; }
        public string Comment { get; set; }
    }
}