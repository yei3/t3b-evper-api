using System.Collections.Generic;
using Yei3.PersonalEvaluation.Evaluations.Terms;

namespace Yei3.PersonalEvaluation.Dashboard.Dto
{
    public class SupervisorUserDashboardDto
    {
        public EvaluationTerm NextEvaluationTerm { get; set; }
        public SupervisorToDoes SupervisorToDoes { get; set; }
        public ICollection<EvaluationSummaryDto> CollaboratorsEvaluationSummary { get; set; }
        public ICollection<RevisionSummaryDto> CollaboratorRevisionSummary { get; set; }
        public ICollection<CollaboratorsObjectivesSummaryDto> CollaboratorsObjectivesSummary { get; set; }
        public ICollection<EvaluationActionDto> ActionSummary { get; set; }
    }
}