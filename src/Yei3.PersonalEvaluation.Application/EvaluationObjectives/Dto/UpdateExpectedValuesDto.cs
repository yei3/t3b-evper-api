using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Application.EvaluationObjectives.Dto
{
    public class UpdateExpectedValuesDto : EntityDto<long>
    {
        public decimal? Expected { get; set; }
        public string ExpectedText { get; set; }
    }
}