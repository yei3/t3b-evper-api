namespace Yei3.PersonalEvaluation.Report
{
    public class CollaboratorCompetencesReportDto
    {
        public string CurrentEvaluationTitle { get; set; }
        public int CurrentEvaluationUnsatisfactory { get; set; }
        public int CurrentEvaluationSatisfactory { get; set; }
        public int CurrentEvaluationExceeds { get; set; }

        public string LastEvaluationTitle { get; set; }
        public int LastEvaluationUnsatisfactory { get; set; }
        public int LastEvaluationSatisfactory { get; set; }
        public int LastEvaluationExceeds { get; set; }
    }
}