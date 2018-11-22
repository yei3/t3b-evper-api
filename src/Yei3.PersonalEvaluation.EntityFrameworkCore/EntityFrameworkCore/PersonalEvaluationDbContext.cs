using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.Capabilities;
using Yei3.PersonalEvaluation.Evaluations.Objectives;
using Yei3.PersonalEvaluation.Evaluations.Question;
using Yei3.PersonalEvaluation.Evaluations.Section;
using Yei3.PersonalEvaluation.Identity;
using Yei3.PersonalEvaluation.MultiTenancy;
using Yei3.PersonalEvaluation.OrganizationUnit;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore
{
    public class PersonalEvaluationDbContext : AbpZeroDbContext<Tenant, Role, User, PersonalEvaluationDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<Evaluation> Evaluations { get; set; }
        public virtual DbSet<Objective> Objectives { get; set; }
        public virtual DbSet<EvaluationUserObjective> UserObjectives { get; set; }
        public virtual DbSet<Capability> Capabilities { get; set; }
        public virtual DbSet<EvaluationUserCapability> UserCapabilities { get; set; }
        public virtual DbSet<UserSignature> UserSignatures { get; set; }
        public virtual DbSet<EvaluationUser> EvaluationUsers { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<AreaOrganizationUnit> AreaOrganizationUnits { get; set; }
        public virtual DbSet<RegionOrganizationUnit> RegionOrganizationUnits { get; set; }
        
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
                .HasMany(evaluation => evaluation.Sections)
                .WithOne(capability => capability.Evaluation);

            modelBuilder.Entity<Evaluation>()
                .HasMany(evaluation => evaluation.EvaluationUsers)
                .WithOne(evaluationUser => evaluationUser.Evaluation);

            modelBuilder.Entity<Objective>()
                .HasMany(objective => objective.EvaluationUserObjectives)
                .WithOne(evaluationUserObjective => evaluationUserObjective.Objective);

            modelBuilder.Entity<Capability>()
                .HasMany(capability => capability.EvaluationUserCapabilities)
                .WithOne(evaluationUserCapability => evaluationUserCapability.Capability);

            modelBuilder.Entity<EvaluationUser>()
                .HasMany(evaluationUser => evaluationUser.EvaluationUserObjectives)
                .WithOne(evaluationUserObjective => evaluationUserObjective.EvaluationUser);

            modelBuilder.Entity<EvaluationUser>()
                .HasMany(evaluationUser => evaluationUser.EvaluationUserCapabilities)
                .WithOne(evaluationUserCapability => evaluationUserCapability.EvaluationUser);

            modelBuilder.Entity<User>()
                .HasMany(user => user.UserSignatures)
                .WithOne(userSignature => userSignature.User);
            modelBuilder.Entity<User>()
                .HasMany(user => user.EvaluationsReceived)
                .WithOne(evaluationUser => evaluationUser.User);

            modelBuilder.Entity<UserSignature>()
                .HasMany(userSignature => userSignature.EvaluationUsers)
                .WithOne(evaluationUser => evaluationUser.UserSignature);

            modelBuilder.Entity<Section>()
                .HasMany(section => section.Questions)
                .WithOne(question => question.Section);

            modelBuilder.Entity<Section>()
                .HasOne(section => section.ParentSection);
        }
    }
}
