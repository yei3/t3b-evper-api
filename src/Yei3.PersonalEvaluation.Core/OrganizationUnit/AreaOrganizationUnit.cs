namespace Yei3.PersonalEvaluation.OrganizationUnit
{
    public class AreaOrganizationUnit : Abp.Organizations.OrganizationUnit
    {
        public AreaOrganizationUnit(int? tenantId, string displayName, long? parentId = null) : base(tenantId, displayName,
            parentId)
        {

        }
    }
}