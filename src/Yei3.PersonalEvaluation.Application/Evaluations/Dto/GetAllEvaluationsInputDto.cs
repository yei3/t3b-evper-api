namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.Application.Services.Dto;
    using System;

    public class GetAllEvaluationsInputDto : PagedAndSortedResultRequestDto
    {
        public DateTime? MinTime { get; set; }
        public DateTime? MaxTime { get; set; }
        public long? CreatorUserId { get; set; }
        public long? EvaluatorUserId { get; set; }

    }
}