using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    [AutoMap(typeof(EvaluationMeasuredQuestion))]
    public class EvaluationMeasuredQuestionDto
    {
        public decimal? Expected { get; set; }
        public string ExpectedText { get; set; }
    }
}