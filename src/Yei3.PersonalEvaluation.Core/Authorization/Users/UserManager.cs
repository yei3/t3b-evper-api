namespace Yei3.PersonalEvaluation.Authorization.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Abp.Authorization;
    using Abp.Authorization.Users;
    using Abp.Configuration;
    using Abp.Domain.Entities;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.Organizations;
    using Abp.Runtime.Caching;
    using Yei3.PersonalEvaluation.Authorization.Roles;
    using Yei3.PersonalEvaluation.Core.OrganizationUnit;
    using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;

    public class UserManager : AbpUserManager<Role, User>
    {
        private readonly IRepository<EvaluationRevision, long> _evaluationRevisionRepository;
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
            IRepository<EvaluationRevision, long> evaluationRevisionRepository,
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
            _evaluationRevisionRepository = evaluationRevisionRepository;
        }

        public async Task<User> InactivateAsync(User activeUser, long sessionUserId)
        {

            var sessionUserTask = this.GetUserByIdAsync(sessionUserId);
            var immediateSupervisorTask =
                    this.Users.FirstOrDefaultAsync(user => user.JobDescription == activeUser.ImmediateSupervisor);

            try
            {
                var sessionUser =   await sessionUserTask;                
                // find immediate superior
                var immediateSupervisor = await immediateSupervisorTask;

                if (!immediateSupervisor.IsNullOrDeleted())
                {
                    // throw new UserFriendlyException("No hay supervisor");
                    // find collaborators
                    var collaborators = this.Users
                        .Where(user => user.ImmediateSupervisor == activeUser.JobDescription);

                    // update to new immediate supervisor
                    foreach (var collaborator in collaborators)
                    {
                        collaborator.ImmediateSupervisor = immediateSupervisor.JobDescription;
                    }
                }

                // find dandling revisions
                var pendingRevisions = _evaluationRevisionRepository
                    .GetAll()
                    .Where(evaluationRevision => evaluationRevision.ReviewerUserId == activeUser.Id)
                    .Where(evaluationRevision => evaluationRevision.RevisionDateTime > DateTime.Now)
                    .Where(evaluationRevision => evaluationRevision.Status != EvaluationRevisionStatus.Revised);

                // update new reviewer
                foreach (var pendingRevision in pendingRevisions)
                {
                    pendingRevision.UpdateReviewer(immediateSupervisor);
                }

                //update user
                activeUser.ImmediateSupervisor = String.Empty;
                activeUser.IsActive = false;
                activeUser.DeleterUser = sessionUser;
                activeUser.DeleterUserId = sessionUser.Id;
                activeUser.DeletionTime = DateTime.Now;

                await this.UpdateAsync(activeUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Usuario {activeUser.Id} . Exception: {ex.Message}");
            }

            return activeUser;
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
