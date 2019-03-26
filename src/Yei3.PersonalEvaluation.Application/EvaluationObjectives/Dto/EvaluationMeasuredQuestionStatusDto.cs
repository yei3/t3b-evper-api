using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.EvaluationObjectives.Dto
{
    [AutoMap(typeof(EvaluationMeasuredQuestion))]
    public class EvaluationMeasuredQuestionStatusDto
    {
        public EvaluationQuestionStatus Status { get; set; }
    }
}