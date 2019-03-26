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
    }
}
