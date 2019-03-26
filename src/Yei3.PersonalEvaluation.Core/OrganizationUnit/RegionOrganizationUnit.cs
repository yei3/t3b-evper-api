namespace Yei3.PersonalEvaluation.OrganizationUnit
{
    public class RegionOrganizationUnit : Abp.Organizations.OrganizationUnit
    {
        public RegionOrganizationUnit(int? tenantId, string displayName, long? parentId = null) : base(tenantId, displayName,
            parentId)
        {

        }
    }
}