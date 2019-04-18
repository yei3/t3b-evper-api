using System;
using Abp.AutoMapper;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Yei3.PersonalEvaluation.Report.Dto
{
    public class AdministratorInputDto
    {
        public long? RegionId { get; set; }
        public long? AreaId { get; set; }
        [CanBeNull]
        public string JobDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public long? UserId { get; set; }
    }
}