using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;

namespace Yei3.PersonalEvaluation.Dashboard.Dto
{
    [AutoMap(typeof(EvaluationSummaryValueObject))]
    public class EvaluationSummaryDto : EvaluationSummaryValueObject
    {
        
    }
}