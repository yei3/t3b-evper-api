using System;
using Abp.Domain.Values;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObject
{
    public class ObjectiveBinnacleValueObject : ValueObject<ObjectiveBinnacleValueObject>
    {
        public long Id { get; set; }
        public long EvaluationMeasuredQuestionId { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }

    }
}