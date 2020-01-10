using System.Collections.Generic;

namespace Yei3.PersonalEvaluation.OrganizationUnits.Dto
{
    using Abp.AutoMapper;
    using OrganizationUnit;

    using Abp.Domain.Entities;
    using Yei3.PersonalEvaluation.Core.OrganizationUnit;

    [AutoMap(typeof(Abp.Organizations.OrganizationUnit), typeof(AreaOrganizationUnit), typeof(RegionOrganizationUnit), typeof(SalesAreaOrganizationUnit))]
    public class OrganizationUnitDto : Entity<long>
    {
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public long? ParentId { get; set; }
        public ICollection<OrganizationUnitUserDto> OrganizationUnitUsers { get; set; }
        public bool IsSalesArea { get; set; } = false;

        public OrganizationUnitDto AsSalesArea() {
            this.IsSalesArea = true;
            return this;
        }
    }
}