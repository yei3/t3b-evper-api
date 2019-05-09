using System.Collections.Generic;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Evaluations.Terms;
using Yei3.PersonalEvaluation.Evaluations.ValueObject;

namespace Yei3.PersonalEvaluation.Dashboard.Dto
{
    [AutoMap(typeof(ToDoesSummaryValueObject))]
    public class CollaboratorUserDashboardDto
    {
        public EvaluationTerm NextEvaluationTerm { get; set; }
        public ToDoesSummaryDto ToDoesSummary { get; set; }
        public ICollection<EvaluationSummaryDto> EvaluationSummary { get; set; }
        public ICollection<RevisionSummaryDto> RevisionSummary { get; set; }
        public ICollection<PendingObjectivesSummaryDto> ObjectiveSummary { get; set; }
        public ICollection<EvaluationActionDto> ActionSummary { get; set; }
    }
}