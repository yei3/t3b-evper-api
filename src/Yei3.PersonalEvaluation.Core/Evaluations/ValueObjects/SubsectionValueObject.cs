using Abp.Domain.Values;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObjects
{
    public class SubsectionValueObject: ValueObject<SubsectionValueObject>
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long ParentId { get; set; }
    }
}