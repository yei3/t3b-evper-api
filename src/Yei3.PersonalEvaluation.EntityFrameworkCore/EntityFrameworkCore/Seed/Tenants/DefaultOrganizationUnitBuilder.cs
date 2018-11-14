using System.Linq;
using Abp.Organizations;
using Microsoft.EntityFrameworkCore;

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
            OrganizationUnit organizationUnitRegion = _context.OrganizationUnits.IgnoreQueryFilters()
                .FirstOrDefault(organizationUnit => organizationUnit.Code.Equals("00001"));

            if (organizationUnitRegion != null) return;

            organizationUnitRegion = new OrganizationUnit(_tenantId, "Region");
            organizationUnitRegion.Code = OrganizationUnit.CreateCode(new[] {1});
            _context.OrganizationUnits.Add(organizationUnitRegion);
            _context.SaveChanges();

            OrganizationUnit organizationUnitAreaFinances = new OrganizationUnit(_tenantId, "Finanzas", organizationUnitRegion.Id);
            OrganizationUnit organizationUnitAreaPurchases = new OrganizationUnit(_tenantId, "Compras", organizationUnitRegion.Id);
            OrganizationUnit organizationUnitAreaPurchasesInAndOut = new OrganizationUnit(_tenantId, "Compras In & Out", organizationUnitRegion.Id);
            OrganizationUnit organizationUnitAreaSystemsAndLogistics = new OrganizationUnit(_tenantId, "Sistemas y Logistica", organizationUnitRegion.Id);
            OrganizationUnit organizationUnitAreaSellsAndOperations = new OrganizationUnit(_tenantId, "Ventas y Operaciones", organizationUnitRegion.Id);
            OrganizationUnit organizationUnitAreaHumanResources = new OrganizationUnit(_tenantId, "RRHH", organizationUnitRegion.Id);
            OrganizationUnit organizationUnitAreaExpansion = new OrganizationUnit(_tenantId, "Expansion", organizationUnitRegion.Id);

            organizationUnitAreaFinances.Code = OrganizationUnit.CreateCode(new[] {1, 1});
            organizationUnitAreaPurchases.Code = OrganizationUnit.CreateCode(new[] {1, 2});
            organizationUnitAreaPurchasesInAndOut.Code = OrganizationUnit.CreateCode(new[] {1, 3});
            organizationUnitAreaSystemsAndLogistics.Code = OrganizationUnit.CreateCode(new[] {1, 4});
            organizationUnitAreaSellsAndOperations.Code = OrganizationUnit.CreateCode(new[] {1, 5});
            organizationUnitAreaHumanResources.Code = OrganizationUnit.CreateCode(new[] {1, 6});
            organizationUnitAreaExpansion.Code = OrganizationUnit.CreateCode(new[] {1, 7});

            _context.OrganizationUnits.Add(organizationUnitAreaFinances);
            _context.OrganizationUnits.Add(organizationUnitAreaPurchases);
            _context.OrganizationUnits.Add(organizationUnitAreaPurchasesInAndOut);
            _context.OrganizationUnits.Add(organizationUnitAreaSystemsAndLogistics);
            _context.OrganizationUnits.Add(organizationUnitAreaSellsAndOperations);
            _context.OrganizationUnits.Add(organizationUnitAreaHumanResources);
            _context.OrganizationUnits.Add(organizationUnitAreaExpansion);

            _context.SaveChanges();
        }
    }
}