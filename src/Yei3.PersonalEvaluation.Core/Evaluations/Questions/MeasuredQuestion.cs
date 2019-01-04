using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Sections;

namespace Yei3.PersonalEvaluation.Evaluations.Questions
{
    public class MeasuredQuestion : Question
    {
        public MeasuredQuestion(string text, QuestionType questionType, bool isQualifiable, decimal expected, string expectedText, MeasuredQuestionRelation relation, string deliverable) : base(text, questionType, isQualifiable)
        {
            Expected = expected;
            ExpectedText = expectedText;
            Relation = relation;
            Deliverable = deliverable;
        }

        public MeasuredQuestion()
        {
        }

        public virtual decimal Expected { get; protected set; }
        public virtual string ExpectedText { get; protected set; }
        public virtual MeasuredQuestionRelation Relation { get; protected set; }
        public virtual string Deliverable { get; protected set; }
        public virtual ICollection<EvaluationMeasuredQuestion> EvaluationMeasuredQuestions { get; protected set; }
        [ForeignKey("SectionId")]
        public virtual Section Section { get; protected set; }

    }
}