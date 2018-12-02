using Abp.Domain.Values;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObject
{
    public class CollaboratorsPendingObjectivesSummaryValueObject : ValueObject<CollaboratorsPendingObjectivesSummaryValueObject>
    {
        public string CollaboratorFullName { get; set; }
        public int TotalPendingObjectives { get; set; }
        public int AccomplishedObjectives { get; set; }
    }
}