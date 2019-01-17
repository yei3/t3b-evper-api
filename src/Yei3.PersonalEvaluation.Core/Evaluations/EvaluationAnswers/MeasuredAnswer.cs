using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers
{
    public class MeasuredAnswer : Answer
    {
        public virtual string Observations { get; set; }
        public virtual decimal Real { get; protected set; }
        [ForeignKey("EvaluationQuestionId")]
        public virtual EvaluationMeasuredQuestion EvaluationMeasuredQuestion { get; protected set; }
        public MeasuredAnswer(long evaluationQuestionId) : base(evaluationQuestionId)
        {
        }

        public MeasuredAnswer()
        {
        }
    }
}