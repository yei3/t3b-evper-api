using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Binnacle;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.Questions;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions
{
    public class EvaluationMeasuredQuestion : EvaluationQuestion
    {
        
        public EvaluationMeasuredQuestion(long evaluationId, long evaluationQuestionId, DateTime terminationDateTime, EvaluationQuestionStatus status) : base(evaluationId, evaluationQuestionId, terminationDateTime, status)
        {
        }

        public new void SetAnswer(long questionId)
        {
            MeasuredAnswer = new MeasuredAnswer(questionId);
        }

        [ForeignKey("EvaluationQuestionId")]
        public virtual MeasuredQuestion MeasuredQuestion { get; protected set; }
        public virtual MeasuredAnswer MeasuredAnswer { get; protected set; }
        public virtual ICollection<ObjectiveBinnacle> Binnacle { get; protected set; }

    }
}