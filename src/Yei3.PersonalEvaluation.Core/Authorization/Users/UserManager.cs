namespace Yei3.PersonalEvaluation.Authorization.Users
{

    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Abp.Authorization;
    using Abp.Authorization.Users;
    using Abp.Configuration;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.Organizations;
    using Abp.Runtime.Caching;
    using Yei3.PersonalEvaluation.Authorization.Roles;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using System.Linq;
    using Yei3.PersonalEvaluation.Core;
    using Yei3.PersonalEvaluation.Core.OrganizationUnit;

    public class UserManager : AbpUserManager<Role, User>
    {
        public UserManager(
            RoleManager roleManager,
            UserStore store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOrganizationUnitSettings organizationUnitSettings,
            ISettingManager settingManager)
            : base(
                roleManager,
                store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger,
                permissionManager,
                unitOfWorkManager,
                cacheManager,
                organizationUnitRepository,
                userOrganizationUnitRepository,
                organizationUnitSettings,
                settingManager)
        {
        }

        public async Task<User> FindByEmployeeNumberAsync(string employeeNumber)
        {
            return await this.Users.SingleAsync(users => users.EmployeeNumber == employeeNumber, CancellationToken);
        }

        public async Task<bool> IsUserASalesMan(User user)
        {
            bool isSalesMan = (await GetOrganizationUnitsAsync(user))
                .Any(organizationUnit => organizationUnit is SalesAreaOrganizationUnit);

            return isSalesMan;
        }

        public async Task<List<User>> GetSubordinatesTree(User rootUser)
        {
            List<User> subordinates = Users.Where(user => user.ImmediateSupervisor == rootUser.JobDescription).ToList();
            List<User> result = new List<User>(subordinates);
            foreach (User currentUser in subordinates)
            {
                result.AddRange(await GetSubordinatesTree(currentUser));
            }

            return result;
        }
        
        public async Task<ICollection<User>> GetSubordinates(User rootUser)
        {
            return await Users
                .Where(user => user.ImmediateSupervisor == rootUser.JobDescription)
                .ToListAsync();
        }
    }
}
