using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Yei3.PersonalEvaluation.Report.Dto
{
    public class AdministratorObjectiveReportInputDto
    {
        public List<long> OrganizationUnitId { get; set; }
        [CanBeNull] public string JobDescription { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public long? UserId { get; set; }
    }
}