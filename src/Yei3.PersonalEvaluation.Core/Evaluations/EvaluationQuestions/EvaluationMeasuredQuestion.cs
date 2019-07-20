using System;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.Questions;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions
{
    public class EvaluationMeasuredQuestion : EvaluableQuestion
    {
        public virtual decimal? Expected { get; set; }
        public virtual string ExpectedText { get; set; }
        public virtual string Comment { get; set; }
        public EvaluationMeasuredQuestion(long evaluationId, long evaluationQuestionId, DateTime terminationDateTime, EvaluationQuestionStatus status, decimal? expected, string expectedText, string comment) : base(evaluationId, terminationDateTime, status, evaluationQuestionId)
        {
            Expected = expected;
            ExpectedText = expectedText;
            Comment = comment;
        }

        public new void SetAnswer(long questionId)
        {
            MeasuredAnswer = new MeasuredAnswer(questionId);
        }

        [ForeignKey("EvaluationQuestionId")]
        public virtual MeasuredQuestion MeasuredQuestion { get; protected set; }
        public virtual MeasuredAnswer MeasuredAnswer { get; protected set; }
    }
}