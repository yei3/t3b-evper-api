using System.Linq;
using Abp.Organizations;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.OrganizationUnit;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore.Seed.Tenants
{
    public class DefaultOrganizationUnitBuilder
    {
        private readonly PersonalEvaluationDbContext _context;
        private readonly int _tenantId;

        public DefaultOrganizationUnitBuilder(PersonalEvaluationDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateDefaultOrganizationUnits();
        }

        private void CreateDefaultOrganizationUnits()
        {
            RegionOrganizationUnit organizationUnitRegion = _context.RegionOrganizationUnits.IgnoreQueryFilters()
                .FirstOrDefault(organizationUnit => organizationUnit.Code.Equals("00001"));

            if (organizationUnitRegion != null) return;

            organizationUnitRegion = new RegionOrganizationUnit(_tenantId, "CORPORATIVO")
            {
                Code = Abp.Organizations.OrganizationUnit.CreateCode(new[] {1})
            };
            _context.RegionOrganizationUnits.Add(organizationUnitRegion);
            _context.SaveChanges();

            AreaOrganizationUnit organizationUnitAreaFinances = new AreaOrganizationUnit(_tenantId, "Finanzas", organizationUnitRegion.Id);
            AreaOrganizationUnit organizationUnitAreaPurchases = new AreaOrganizationUnit(_tenantId, "Compras", organizationUnitRegion.Id);
            AreaOrganizationUnit organizationUnitAreaPurchasesInAndOut = new AreaOrganizationUnit(_tenantId, "Compras In & Out", organizationUnitRegion.Id);
            AreaOrganizationUnit organizationUnitAreaSystemsAndLogistics = new AreaOrganizationUnit(_tenantId, "Sistemas y Logistica", organizationUnitRegion.Id);
            AreaOrganizationUnit organizationUnitAreaSellsAndOperations = new AreaOrganizationUnit(_tenantId, "Ventas y Operaciones", organizationUnitRegion.Id);
            AreaOrganizationUnit organizationUnitAreaHumanResources = new AreaOrganizationUnit(_tenantId, "RECURSOS HUMANOS", organizationUnitRegion.Id);
            AreaOrganizationUnit organizationUnitAreaExpansion = new AreaOrganizationUnit(_tenantId, "Expansion", organizationUnitRegion.Id);

            organizationUnitAreaFinances.Code = Abp.Organizations.OrganizationUnit.CreateCode(new[] {1, 1});
            organizationUnitAreaPurchases.Code = Abp.Organizations.OrganizationUnit.CreateCode(new[] {1, 2});
            organizationUnitAreaPurchasesInAndOut.Code = Abp.Organizations.OrganizationUnit.CreateCode(new[] {1, 3});
            organizationUnitAreaSystemsAndLogistics.Code = Abp.Organizations.OrganizationUnit.CreateCode(new[] {1, 4});
            organizationUnitAreaSellsAndOperations.Code = Abp.Organizations.OrganizationUnit.CreateCode(new[] {1, 5});
            organizationUnitAreaHumanResources.Code = Abp.Organizations.OrganizationUnit.CreateCode(new[] {1, 6});
            organizationUnitAreaExpansion.Code = Abp.Organizations.OrganizationUnit.CreateCode(new[] {1, 7});

            _context.AreaOrganizationUnits.Add(organizationUnitAreaFinances);
            _context.AreaOrganizationUnits.Add(organizationUnitAreaPurchases);
            _context.AreaOrganizationUnits.Add(organizationUnitAreaPurchasesInAndOut);
            _context.AreaOrganizationUnits.Add(organizationUnitAreaSystemsAndLogistics);
            _context.AreaOrganizationUnits.Add(organizationUnitAreaSellsAndOperations);
            _context.AreaOrganizationUnits.Add(organizationUnitAreaHumanResources);
            _context.AreaOrganizationUnits.Add(organizationUnitAreaExpansion);

            _context.SaveChanges();
        }
    }
}