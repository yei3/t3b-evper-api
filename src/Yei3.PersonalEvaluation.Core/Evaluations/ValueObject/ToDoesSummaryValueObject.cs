using Abp.Domain.Values;

namespace Yei3.PersonalEvaluation.Evaluations.ValueObject
{
    public class ToDoesSummaryValueObject : ValueObject<ToDoesSummaryValueObject>
    {
        public int AutoEvaluations { get; set; }
        public int Evaluations { get; set; }
        public int Objectives { get; set; }
    }
}