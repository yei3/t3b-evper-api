using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Yei3.PersonalEvaluation.Configuration;
using Yei3.PersonalEvaluation.Web;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class PersonalEvaluationDbContextFactory : IDesignTimeDbContextFactory<PersonalEvaluationDbContext>
    {
        public PersonalEvaluationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PersonalEvaluationDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            PersonalEvaluationDbContextConfigurer.Configure(builder, configuration.GetConnectionString(PersonalEvaluationConsts.ConnectionStringName));

            return new PersonalEvaluationDbContext(builder.Options);
        }
    }
}
