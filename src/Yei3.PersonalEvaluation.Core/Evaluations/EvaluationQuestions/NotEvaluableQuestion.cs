using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Binnacle;
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

        public virtual long SectionId { get; protected set; }
        [ForeignKey("SectionId")]
        public virtual Section Section { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual NotEvaluableAnswer NotEvaluableAnswer { get; protected set; }
        public virtual ICollection<ObjectiveBinnacle> Binnacle { get; protected set; }
        public void SetAnswer(long questionId)
        {
            NotEvaluableAnswer = new NotEvaluableAnswer(questionId);
        }
    }
}