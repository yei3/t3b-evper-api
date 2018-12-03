using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;

namespace Yei3.PersonalEvaluation.Dashboard.Dto
{
    [AutoMap(typeof(EvaluationObjectivesSummaryValueObject))]
    public class PendingObjectivesSummaryDto : EvaluationObjectivesSummaryValueObject
    {
        
    }
}