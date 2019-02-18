using System;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationAnswers;
using Yei3.PersonalEvaluation.Evaluations.Sections;

namespace Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions
{
    public class NotEvaluableQuestion : EvaluationQuestion
    {
        public NotEvaluableQuestion()
        {
        }

        public NotEvaluableQuestion(long sectionId, string text)
        {
            SectionId = sectionId;
            Text = text;
        }

        public NotEvaluableQuestion(long sectionId, string text, long evaluationId, DateTime terminationDateTime, EvaluationQuestionStatus status): base(evaluationId, terminationDateTime, status)
        {
            SectionId = sectionId;
            Text = text;
        }

        public virtual long SectionId { get; set; }
        [ForeignKey("SectionId")]
        public virtual Section Section { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual NotEvaluableAnswer NotEvaluableAnswer { get; protected set; }
        public void SetAnswer(long questionId, string text, DateTime terminationDateTime)
        {
            NotEvaluableAnswer = new NotEvaluableAnswer(questionId, text, terminationDateTime);
        }
    }
}