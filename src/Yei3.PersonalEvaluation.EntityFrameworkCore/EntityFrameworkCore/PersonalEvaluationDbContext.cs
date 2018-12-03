using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations;
using Yei3.PersonalEvaluation.Evaluations.EvaluationQuestions;
using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;
using Yei3.PersonalEvaluation.Evaluations.EvaluationTemplates;
using Yei3.PersonalEvaluation.Evaluations.Questions;
using Yei3.PersonalEvaluation.Evaluations.Sections;
using Yei3.PersonalEvaluation.MultiTenancy;
using Yei3.PersonalEvaluation.OrganizationUnit;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore
{
    public class PersonalEvaluationDbContext : AbpZeroDbContext<Tenant, Role, User, PersonalEvaluationDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<EvaluationTemplate> EvaluationTemplates { get; set; }
        public virtual DbSet<Evaluation> Evaluation { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<UnmeasuredQuestion> Questions { get; set; }
        public virtual DbSet<MeasuredQuestion> MeasuredQuestions { get; set; }
        public virtual DbSet<Question> FrequentQuestions { get; set; }
        public virtual DbSet<EvaluationQuestion> EvaluationQuestions { get; set; }
        public virtual DbSet<AreaOrganizationUnit> AreaOrganizationUnits { get; set; }
        public virtual DbSet<RegionOrganizationUnit> RegionOrganizationUnits { get; set; }
        public virtual DbSet<EvaluationRevision> EvaluationRevisions{ get; set; }
        
        public PersonalEvaluationDbContext(DbContextOptions<PersonalEvaluationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EvaluationTemplate>()
                .HasMany(evaluationTemplate => evaluationTemplate.Evaluations)
                .WithOne(evaluation => evaluation.Template);

            modelBuilder.Entity<EvaluationTemplate>()
                .HasMany(evaluationTemplate => evaluationTemplate.Sections)
                .WithOne(section => section.Template);

            modelBuilder.Entity<Section>()
                .HasMany(section => section.UnmeasuredQuestions)
                .WithOne(question => question.Section);

            modelBuilder.Entity<User>()
                .HasMany(user => user.EvaluationsReceived)
                .WithOne(evaluationUser => evaluationUser.User);

            modelBuilder.Entity<Section>()
                .HasMany(section => section.UnmeasuredQuestions)
                .WithOne(question => question.Section);

            modelBuilder.Entity<Section>()
                .HasMany(section => section.MeasuredQuestions)
                .WithOne(question => question.Section);

            modelBuilder.Entity<Section>()
                .HasOne(x => x.ParentSection).WithMany(x => x.ChildSections)
                .Metadata.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<Section>()
                .HasMany(section => section.ChildSections)
                .WithOne(section => section.ParentSection);

            modelBuilder.Entity<UnmeasuredQuestion>()
                .HasMany(question => question.EvaluationQuestions)
                .WithOne(evaluationQuestion => evaluationQuestion.UnmeasuredQuestion);

            modelBuilder.Entity<MeasuredQuestion>()
                .HasMany(question => question.EvaluationQuestions)
                .WithOne(evaluationQuestion => evaluationQuestion.MeasuredQuestion);

            modelBuilder.Entity<EvaluationQuestion>()
                .HasOne(evaluationQuestion => evaluationQuestion.Answer)
                .WithOne(answer => answer.EvaluationQuestion);

            modelBuilder.Entity<Evaluation>()
                .HasMany(evaluation => evaluation.Questions)
                .WithOne(question => question.Evaluation);

            modelBuilder.Entity<Evaluation>()
                .HasOne(evaluation => evaluation.Revision)
                .WithOne(revision => revision.Evaluation);

            modelBuilder.Entity<EvaluationRevision>()
                .HasOne(evaluation => evaluation.ReviewerUser)
                .WithMany(user => user.EvaluationRevisions);
        }
    }
}
