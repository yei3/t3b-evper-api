using System;
using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    public class EvaluationStatusInputDto : PagedResultRequestDto
    {
        public bool ApplyPagination { get; set; } = true;
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
}