namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(Evaluation))]
    public class EvaluationCloseDto : EntityDto<long>
    {
        public string Comment { get; set; }
    }
}