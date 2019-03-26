using Yei3.PersonalEvaluation.Evaluations.Terms;

namespace Yei3.PersonalEvaluation.Report.Dto
{
    public class UserEvaluationsComparisonInputDto
    {
        public long LeftEvaluationTemplateId { get; set; }
        public EvaluationTerm LeftEvaluationTerm { get; set; }
        public int LeftEvaluationDayOfYear { get; set; }
        public long RightEvaluationTemplateId { get; set; }
        public EvaluationTerm RightEvaluationTerm { get; set; }
        public int RightEvaluationYear { get; set; }
    }
}