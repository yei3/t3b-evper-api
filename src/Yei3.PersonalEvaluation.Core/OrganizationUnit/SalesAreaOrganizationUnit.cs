using Yei3.PersonalEvaluation.OrganizationUnit;

namespace Yei3.PersonalEvaluation.Core.OrganizationUnit
{
    public class SalesAreaOrganizationUnit : AreaOrganizationUnit
    {
        public SalesAreaOrganizationUnit(int? tenantId, string displayName, long? parentId = null) : base(tenantId, displayName,
            parentId)
        {

        }
    }
}