namespace Yei3.PersonalEvaluation.OrganizationUnits.Dto
{
    using Abp.Domain.Entities;

    public class OrganizationUnitDto : Entity<long>
    {
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public long? ParentId { get; set; }
    }
}