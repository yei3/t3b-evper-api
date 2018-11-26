using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Question.Dto
{
    public class QuestionGetAllInputDto : PagedAndSortedResultRequestDto
    {
        public long SectionId { get; set; }
    }
}