using System;
using Abp.Domain.Values;
using Yei3.PersonalEvaluation.Evaluations.Terms;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObject
{
    public class EvaluationSummaryValueObject : ValueObject<EvaluationSummaryValueObject>
    {
        public long Id { get; set; }
        public EvaluationStatus Status { get; set; }
        public EvaluationTerm Term { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CollaboratorName { get; set; }
    }
}