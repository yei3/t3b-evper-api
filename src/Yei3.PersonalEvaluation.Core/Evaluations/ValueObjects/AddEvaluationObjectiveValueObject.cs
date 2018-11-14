using System;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;
    using Interfaces;

    public class AddEvaluationObjectiveValueObject : ValueObject<AddEvaluationObjectiveValueObject>, IDescribed, IIndexed
    {
        public long EvaluationId { get; set; }
        public byte Index { get; set; }
        public string Description { get; set; }
        public string DefinitionOfDone { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}