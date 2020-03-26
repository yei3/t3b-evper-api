using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    public class EvaluationResultListItemDto : EntityDto<long>
    {
        public string EmployeeNumber { get; set; }
        public string Period { get; set; }
        public decimal AchievedObjectivesPercent { get; set; }
        public decimal ExceededRequirementPercent { get; set; }
        public decimal CompletedRequirementPercent { get; set; }
        public decimal UnsatisfactoryRequirementPercent { get; set; }
        public int CurrentObjectivesCount { get; set; }
        public decimal GlobalEvaluationsCount { get; set; }
    }
}