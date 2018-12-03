namespace Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers
{
    public class EvaluationMeasuredAnswer : EvaluationAnswer
    {
        public virtual decimal Real { get; protected set; }

        public EvaluationMeasuredAnswer(long evaluationQuestionId, long evaluationId) : base(evaluationQuestionId, evaluationId)
        {
        }
    }
}