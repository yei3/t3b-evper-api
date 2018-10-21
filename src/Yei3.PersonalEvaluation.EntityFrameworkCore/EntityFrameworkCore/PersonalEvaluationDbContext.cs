using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.Capabilities;
using Yei3.PersonalEvaluation.Evaluations.Objectives;
using Yei3.PersonalEvaluation.Identity;
using Yei3.PersonalEvaluation.MultiTenancy;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore
{
    public class PersonalEvaluationDbContext : AbpZeroDbContext<Tenant, Role, User, PersonalEvaluationDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<Evaluation> Evaluations { get; set; }
        public virtual DbSet<Objective> Objectives { get; set; }
        public virtual DbSet<UserObjective> UserObjectives { get; set; }
        public virtual DbSet<Capability> Capabilities { get; set; }
        public virtual DbSet<UserCapability> UserCapabilities { get; set; }
        public virtual DbSet<UserSignature> UserSignatures { get; set; }
        
        public PersonalEvaluationDbContext(DbContextOptions<PersonalEvaluationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Evaluation>()
                .HasMany(evaluation => evaluation.Objectives)
                .WithOne(objective => objective.Evaluation);
            modelBuilder.Entity<Evaluation>()
                .HasMany(evaluation => evaluation.Capabilities)
                .WithOne(capability => capability.Evaluation);
            modelBuilder.Entity<Evaluation>()
                .HasMany(evaluation => evaluation.NextTermObjectives)
                .WithOne(objective => objective.NextEvaluation);

            modelBuilder.Entity<Objective>()
                .HasMany(objective => objective.UserObjectives)
                .WithOne(userObjective => userObjective.Objective);

            modelBuilder.Entity<Capability>()
                .HasMany(capability => capability.UserCapabilities)
                .WithOne(userCapability => userCapability.Capability);

            modelBuilder.Entity<User>()
                .HasMany(user => user.UserSignatures)
                .WithOne(userSignature => userSignature.User);
            modelBuilder.Entity<User>()
                .HasMany(user => user.UserObjectives)
                .WithOne(userObjective => userObjective.User);
            modelBuilder.Entity<User>()
                .HasMany(user => user.UserCapabilities)
                .WithOne(userCapability => userCapability.User);
            modelBuilder.Entity<User>()
                .HasMany(user => user.EvaluationsPerformed)
                .WithOne(evaluation => evaluation.EvaluatorUser);
            modelBuilder.Entity<User>()
                .HasMany(user => user.EvaluationsReceived)
                .WithOne(evaluation => evaluation.EvaluatedUser);
        }
    }
}
