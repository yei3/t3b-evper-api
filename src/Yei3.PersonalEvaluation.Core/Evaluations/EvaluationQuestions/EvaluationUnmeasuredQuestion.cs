using System;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.Questions;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions
{
    public class EvaluationUnmeasuredQuestion : EvaluationQuestion
    {
        public EvaluationUnmeasuredQuestion(long evaluationId, long evaluationQuestionId, DateTime terminationDateTime, EvaluationQuestionStatus status) : base(evaluationId, evaluationQuestionId, terminationDateTime, status)
        {
        }

        public new void SetAnswer(long questionId)
        {
            UnmeasuredAnswer = new UnmeasuredAnswer(questionId);
        }

        [ForeignKey("EvaluationQuestionId")]
        public virtual UnmeasuredQuestion UnmeasuredQuestion { get; protected set; }
        public virtual UnmeasuredAnswer UnmeasuredAnswer { get; protected set; }
    }
}