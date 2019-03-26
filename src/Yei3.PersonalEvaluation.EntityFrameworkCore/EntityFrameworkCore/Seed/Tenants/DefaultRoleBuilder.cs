using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Localization;
using Yei3.PersonalEvaluation.Authorization;
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

            List<Permission> permissions = new List<Permission>
            {
                new Permission(PermissionNames.Pages_Users, new LocalizableString(PermissionNames.Pages_Users, PermissionNames.Pages_Users)),
                new Permission(PermissionNames.Pages_Roles, new LocalizableString(PermissionNames.Pages_Roles, PermissionNames.Pages_Roles)),

            };

            _context.Permissions.AddRange(
                permissions.Select(permission => new RolePermissionSetting
                {
                    TenantId = _tenantId,
                    Name = permission.Name,
                    IsGranted = true,
                    RoleId = administratorRole.Id
                })
            );
            _context.SaveChanges();
        }
    }
}
