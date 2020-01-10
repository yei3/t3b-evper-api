using Abp.Application.Services.Dto;
using Yei3.PersonalEvaluation.Evaluations;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    public class EvaluationStatusListItemDto : EntityDto<long>
    {
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public string EvaluationName { get; set; }
        public bool IsAutoEvaluation { get; set; }
        public bool IncludePastObjectives { get; set; }
        public EvaluationStatus Status { get; set; }
    }
}