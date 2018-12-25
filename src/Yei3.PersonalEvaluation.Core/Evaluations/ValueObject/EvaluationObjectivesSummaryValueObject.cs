using System;
using Abp.Domain.Values;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObject
{
    public class EvaluationObjectivesSummaryValueObject : ValueObject<EvaluationObjectivesSummaryValueObject>
    {
        public EvaluationQuestionStatus Status { get; set; }
        public string Name { get; set; }
        public string Deliverable { get; set; }
        public DateTime DeliveryDate { get; set; }
        public long Id { get; set; }
    }
}