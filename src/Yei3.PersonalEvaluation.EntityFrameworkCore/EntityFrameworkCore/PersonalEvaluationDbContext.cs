using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.MultiTenancy;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore
{
    public class PersonalEvaluationDbContext : AbpZeroDbContext<Tenant, Role, User, PersonalEvaluationDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public PersonalEvaluationDbContext(DbContextOptions<PersonalEvaluationDbContext> options)
            : base(options)
        {
        }
    }
}
