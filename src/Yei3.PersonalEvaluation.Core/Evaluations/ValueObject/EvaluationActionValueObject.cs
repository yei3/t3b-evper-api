using System;
using System.Collections.Generic;
using Abp.Domain.Values;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObject
{
    public class EvaluationActionValueObject : ValueObject<EvaluationActionValueObject>
    {
        public string Description { get; set; }
        public string Responsible { get; set; }
        public DateTime DeliveryDate { get; set; }
        public long Id { get; set; }
    }
}