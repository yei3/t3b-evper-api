using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Binnacle.Dto
{
    public class ObjectiveBinnacleGetAllInputDto : PagedAndSortedResultRequestDto
    {
        public long EvaluationMeasuredQuestionId { get; set; }
    }
}