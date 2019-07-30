using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Application.EvaluationObjectives.Dto
{
    public class UpdateExpectedValuesDto : EntityDto<long>
    {
        public decimal? ExpectedAnswer { get; set; }
        public string ExpectedAnswerText { get; set; }
        public decimal? ExpectedQuestion { get; set; }
        public string ExpectedQuestionText { get; set; }
        public string Comment { get; set; }
        public int? Status { get; set; }
    }
}