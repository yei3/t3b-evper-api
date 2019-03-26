using System.Linq;
using Microsoft.EntityFrameworkCore;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations;

namespace Yei3.PersonalEvaluation.EntityFrameworkCore.Seed.Tenants
{
    public class DefaultEvaluationAndObjectivesBuilder
    {
        private readonly PersonalEvaluationDbContext _context;
        private readonly int _tenantId;

        public DefaultEvaluationAndObjectivesBuilder(PersonalEvaluationDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateDefaultObjectives();
        }

        private void CreateDefaultObjectives()
        {
            //Evaluation defaultEvaluation = _context.Evaluations.IgnoreQueryFilters().FirstOrDefault();

            //if (defaultEvaluation != null) return;

            //User evaluatorUser = _context.Users.FirstOrDefault(user => user.UserName == "50328");
            //User evaluatedUser = _context.Users.FirstOrDefault(user => user.UserName == "50885");

            //if(evaluatorUser == null) return;
            
            //defaultEvaluation = new Evaluation();

        }
    }
}