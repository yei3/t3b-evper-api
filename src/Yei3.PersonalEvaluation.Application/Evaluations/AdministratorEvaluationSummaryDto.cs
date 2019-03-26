using System;

namespace Yei3.PersonalEvaluation.Evaluations
{
    public class AdministratorEvaluationSummaryDto
    {
        public long EvaluationTemplateId { get; set; }
        public EvaluationStatus Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}