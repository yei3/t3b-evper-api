using System;
using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.EvaluationTemplate.Dto
{
    public class GetAllEvaluationTemplateInputDto : PagedAndSortedResultRequestDto
    {
        public DateTime? MinTime { get; set; }
        public DateTime? MaxTime { get; set; }
        public long? CreatorUserId { get; set; }
    }
}