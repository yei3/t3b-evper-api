using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore
{
    public static class PersonalEvaluationDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<PersonalEvaluationDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<PersonalEvaluationDbContext> builder, DbConnection connection)
        {
            builder.UseCosmosSql("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", "PersonalEvaluation");
        }
    }
}
