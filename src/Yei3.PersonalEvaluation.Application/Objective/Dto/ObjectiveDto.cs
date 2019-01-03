using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;

namespace Yei3.PersonalEvaluation.Objective.Dto
{
    [AutoMap(typeof(MeasuredQuestion))]
    public class ObjectiveDto : QuestionDto
    {
        public decimal Expected { get; set; }
        public string ExpectedText { get; set; }
        public MeasuredQuestionRelation Relation { get; set; }
    }
}