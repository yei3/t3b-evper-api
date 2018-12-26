using System;
using Abp.Domain.Values;
using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;
using Yei3.PersonalEvaluation.Evaluations.Terms;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObject
{
    public class RevisionSummaryValueObject : ValueObject<RevisionSummaryValueObject>
    {
        public long EvaluationId { get; set; }
        public EvaluationRevisionStatus Status { get; set; }
        public EvaluationTerm Term { get; set; }
        public string Name { get; set; }
        public string CollaboratorFullName { get; set; }
        public string Description { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime RevisionDateTime { get; set; }
    }
}