using System;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers
{
    public class UnmeasuredAnswer : Answer
    {
        [ForeignKey("EvaluationQuestionId")]
        public virtual EvaluationUnmeasuredQuestion EvaluationUnmeasuredQuestion{ get; protected set; }
        public string Action { get; set; }
        public DateTime CommitmentTime { get; set; }

        public UnmeasuredAnswer(long evaluationQuestionId) : base(evaluationQuestionId)
        {
        }

        public UnmeasuredAnswer()
        {
        }
    }
}