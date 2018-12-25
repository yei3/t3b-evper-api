using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.EvaluationAnswer.Dto
{
    [AutoMap(typeof(EvaluationQuestion))]
    public class UnmeasuredEvaluationQuestionStatusDto
    {
        public EvaluationQuestionStatus Status { get; set; }
    }
}