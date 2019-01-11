using System;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions
{
    public abstract class EvaluableQuestion : EvaluationQuestion
    {
        protected EvaluableQuestion(long evaluationId, DateTime terminationDateTime, EvaluationQuestionStatus status, long evaluationQuestionId): base(evaluationId, terminationDateTime, status)
        {
            EvaluationQuestionId = evaluationQuestionId;
        }

        public void SetAnswer(long questionId)
        {

        }
        public virtual long EvaluationQuestionId { get; protected set; }

    }
}