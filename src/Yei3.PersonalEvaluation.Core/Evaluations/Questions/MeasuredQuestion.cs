using Abp.Domain.Entities.Auditing;

namespace Yei3.PersonalEvaluation.Evaluations.Questions
{
    public class MeasuredQuestion : UnmeasuredQuestion
    {
        public virtual decimal Expected { get; protected set; }
        public virtual string Deliverable { get; protected set; }

    }
}