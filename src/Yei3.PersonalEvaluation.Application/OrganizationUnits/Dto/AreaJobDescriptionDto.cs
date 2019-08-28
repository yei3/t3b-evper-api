using System.Collections.Generic;

namespace Yei3.PersonalEvaluation.OrganizationUnits.Dto
{
    public class AreaJobDescriptionDto
    {
        public ICollection<string> JobDescriptions { get; set; }
        public long AreaId { get; set; }
    }
}