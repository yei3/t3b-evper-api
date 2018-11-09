namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.AutoMapper;
    using Abp.Application.Services.Dto;
    using Term;
    using System;

    [AutoMap(typeof(Evaluation))]
    public class EvaluationDto : EntityDto<long>
    {
        public EvaluationUserDto EvaluatorUser { get; set; }
        public DateTime CreationTime { get; set; }
        public EvaluationTerm Term { get; set; }
    }
}