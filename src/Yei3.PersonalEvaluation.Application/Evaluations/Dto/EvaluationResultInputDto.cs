using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    public class EvaluationResultInputDto : PagedResultRequestDto
    {
        public bool ApplyPagination { get; set; } = true;
        [Required]
        public DateTime? StartDateTime { get; set; }
        [Required]
        public DateTime? EndDateTime { get; set; }
    }
}