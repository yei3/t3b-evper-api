using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Sections;

namespace Yei3.PersonalEvaluation.Evaluations.Questions
{
    public class UnmeasuredQuestion : Question
    {
        public UnmeasuredQuestion(string text, QuestionType questionType, bool isQualifiable) : base(text, questionType, isQualifiable)
        {
        }

        public virtual ICollection<EvaluationUnmeasuredQuestion> EvaluationUnmeasuredQuestions{ get; protected set; }
        [ForeignKey("SectionId")]
        public virtual Section Section { get; protected set; }
    }
}