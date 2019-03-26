namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.Application.Services.Dto;

    public class SetEvaluationInstructionsDto : EntityDto<long>
    {
        public string Instructions { get; set; }
    }
}