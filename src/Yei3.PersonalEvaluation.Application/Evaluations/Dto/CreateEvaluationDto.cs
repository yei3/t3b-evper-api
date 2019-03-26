using System;
using System.Collections.Generic;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;

    [AutoMap(typeof(Evaluation))]
    public class CreateEvaluationDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long EvaluationTemplateId { get; set; }
        public ICollection<long> OrganizationUnitIds { get; set; }
        public ICollection<string> JobDescriptions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}