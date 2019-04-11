using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Yei3.PersonalEvaluation.Report.Dto
{
    public class AdministratorObjectiveReportInputDto
    {
        public ICollection<long> OrganizationUnitId { get; set; }
        [CanBeNull]
        public string JobDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public long? UserId { get; set; }
    }
}