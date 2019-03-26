using System;
using System.Collections.Generic;
using Yei3.PersonalEvaluation.Evaluations.Terms;

namespace Yei3.PersonalEvaluation.Report.Dto
{
    public class EvaluationReportDto
    {
        public string Name { get; set; }
        public EvaluationTerm Term { get; set; }
        public DateTime CreationTime { get; set; }
        public int AntiquityAverage { get; set; }
        public int TotalEmployees { get; set; }
        public int EvaluatedEmployees { get; set; }
        public ICollection<SectionSummaryDto> Sections { get; set; }
    }
}