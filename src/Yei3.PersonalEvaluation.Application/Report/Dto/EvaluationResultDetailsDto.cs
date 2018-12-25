using System;
using Yei3.PersonalEvaluation.Evaluations.Terms;

namespace Yei3.PersonalEvaluation.Report.Dto
{
    public class EvaluationResultDetailsDto
    {
        public int AntiquityAverage { get; set; }
        public string EvaluationName { get; set; }
        public string EvaluationDescription { get; set; }
        public int TotalEmployees { get; set; }
        public int EvaluatedEmployees { get; set; }
        public EvaluationTerm Term { get; set; }
        public DateTime CreationTime { get; set; }
        public int FinishedEvaluations { get; set; }
        public DateTime PreviousEvaluationCreationTime { get; set; }
        public EvaluationTerm PreviousEvaluationTerm { get; set; }
        public int PreviousEvaluationEvaluatedEmployees { get; set; }
        public int PreviousEvaluationFinishedEvaluations { get; set; }
    }
}