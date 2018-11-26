using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Section.Dto
{
    public class SectionGetAllInputDto : PagedAndSortedResultRequestDto
    {
        public long EvaluationTemplateId { get; set; }
    }
}