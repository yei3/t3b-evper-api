namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using ValueObjects;
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(Evaluation))]
    public class CreateEvaluationDto : NewEvaluationValueObject, IEntityDto<long>
    {
        public long Id { get; set; }
    }
}