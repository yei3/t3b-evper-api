using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore
{
    public static class PersonalEvaluationDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<PersonalEvaluationDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<PersonalEvaluationDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection);
        }
    }
}
