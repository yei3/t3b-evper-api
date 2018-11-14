using Microsoft.EntityFrameworkCore;
using System.Linq;
using Yei3.PersonalEvaluation.Authorization.Roles;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore.Seed.Tenants
{
    class DefaultRoleBuilder
    {
        private readonly PersonalEvaluationDbContext _context;
        private readonly int _tenantId;

        public DefaultRoleBuilder(PersonalEvaluationDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateDefaultRoles();
        }

        private void CreateDefaultRoles()
        {
            // Default roles

            Role collaboratorRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(role => role.Name == StaticRoleNames.Tenants.Collaborator);

            if (collaboratorRole != null) return;

            collaboratorRole = new Role(_tenantId, StaticRoleNames.Tenants.Collaborator, StaticRoleNames.Tenants.Collaborator);
            Role administratorRole = new Role(_tenantId, StaticRoleNames.Tenants.Administrator, StaticRoleNames.Tenants.Administrator);
            Role supervisorRole = new Role(_tenantId, StaticRoleNames.Tenants.Supervisor, StaticRoleNames.Tenants.Supervisor);

            _context.Roles.Add(collaboratorRole);
            _context.Roles.Add(administratorRole);
            _context.Roles.Add(supervisorRole);
            _context.SaveChanges();
        }
    }
}
