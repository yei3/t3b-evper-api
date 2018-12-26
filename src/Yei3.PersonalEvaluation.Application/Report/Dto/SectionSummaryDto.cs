using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Report.Dto
{
    public class SectionSummaryDto : EntityDto<long>
    {
        public string Name { get; set; }
        public int NonAnsweredQuestions { get; set; }
        public int NonFinishedQuestions { get; set; }
        public int FinishedQuestions { get; set; }
    }
}