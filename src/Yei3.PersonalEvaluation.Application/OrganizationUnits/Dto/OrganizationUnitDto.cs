namespace Yei3.PersonalEvaluation.OrganizationUnits.Dto
{
    using Abp.AutoMapper;
    using OrganizationUnit;

    using Abp.Domain.Entities;
    [AutoMap(typeof(Abp.Organizations.OrganizationUnit), typeof(AreaOrganizationUnit), typeof(RegionOrganizationUnit))]
    public class OrganizationUnitDto : Entity<long>
    {
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public long? ParentId { get; set; }
    }
}