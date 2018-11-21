
namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    using Abp.Domain.Values;
    using System.Collections.Generic;
    public class SectionValueObject : ValueObject<SectionValueObject>
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long EvaluationId { get; set; }
    }
}