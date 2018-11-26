namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(Evaluation))]
    public class CreateEvaluationDto : IEntityDto<long>
    {
        public long Id { get; set; }
    }
}