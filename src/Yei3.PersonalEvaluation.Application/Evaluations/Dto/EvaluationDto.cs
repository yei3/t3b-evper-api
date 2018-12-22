using System.Collections.Generic;
using Yei3.PersonalEvaluation.Evaluations.Terms;
using Yei3.PersonalEvaluation.EvaluationTemplate.Dto;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.AutoMapper;
    using Abp.Application.Services.Dto;
    using System;

    [AutoMap(typeof(Evaluation))]
    public class EvaluationDto : EntityDto<long>
    {
        public EvaluationUserDto User { get; set; }
        public DateTime StartDateTime { get; set; }
        public EvaluationTerm Term { get; set; }
        public EvaluationStatus Status { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Name { get; set; }
        public EvaluationTemplateDto Template { get; set; }
        public ICollection<EvaluationQuestionDto> Questions { get; set; }
    }
}