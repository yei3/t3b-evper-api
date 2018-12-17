using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.Sections;

namespace Yei3.PersonalEvaluation.Evaluations.Questions
{
    public class MeasuredQuestion : Question
    {
        public virtual decimal Expected { get; protected set; }
        public virtual MeasuredQuestionRelation Relation { get; protected set; }
        public virtual string Deliverable { get; protected set; }
        public virtual long SectionId { get; protected set; }
        [ForeignKey("SectionId")]
        public virtual Section Section { get; protected set; }
        public virtual ICollection<EvaluationQuestion> EvaluationQuestions { get; protected set; }

    }
}