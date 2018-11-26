namespace Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers
{
    public class EvaluationMeasuredAnswer : EvaluationAnswer
    {
        public virtual decimal Expected { get; protected set; }
        public virtual decimal Real { get; protected set; }
    }
}