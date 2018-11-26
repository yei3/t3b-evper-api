using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Sections;

namespace Yei3.PersonalEvaluation.Evaluations.Questions
{
    public class Question : FrequentQuestion
    {
        public virtual long SectionId { get; protected set; }
        [ForeignKey("SectionId")]
        public virtual Section Section { get; protected set; }
        public virtual ICollection<EvaluationQuestion> EvaluationQuestions { get; protected set; }
    }
}