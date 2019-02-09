namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Yei3.PersonalEvaluation.Authorization.Users;

    [AutoMap(typeof(Evaluation))]
    public class EvaluationCloseDto : EntityDto<long>
    {
        public long EvaluationId { get; set; }
        public string Comment { get; set; }
    }
}