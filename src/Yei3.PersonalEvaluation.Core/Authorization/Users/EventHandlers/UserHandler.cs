using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;

namespace Yei3.PersonalEvaluation.Core.Authorization.Users.EventHandlers
{
    public class UserHandler : IEventHandler<EntityDeletingEventData<User>>, ITransientDependency
    {

        private readonly UserManager _userManager;
        private readonly IRepository<EvaluationRevision, long> _evaluationRevisionRepository;

        public UserHandler(
            UserManager userManager,
            IRepository<EvaluationRevision, long> evaluationRevisionRepository
        ) {
            _userManager = userManager;
            _evaluationRevisionRepository = evaluationRevisionRepository;
        }

        public void HandleEvent(EntityDeletingEventData<User> eventData)
        {
            // find user immediate superior
            User immediateSupervisor = _userManager.Users.Single(user => user.JobDescription == eventData.Entity.ImmediateSupervisor);

            // find subordinates
            IQueryable<User> subordinates = _userManager.Users
                .Where(user => user.ImmediateSupervisor == eventData.Entity.JobDescription);
            
            // update to new immediate supervisor
            foreach (User subordinate in subordinates)
            {
                subordinate.ImmediateSupervisor = immediateSupervisor.JobDescription;
            }

            // find dandling revisions
            IQueryable<EvaluationRevision> pendingRevisions = _evaluationRevisionRepository
                .GetAll()
                .Where(evaluationRevision => evaluationRevision.ReviewerUserId == eventData.Entity.Id);
            
            // update new reviewer
            foreach (EvaluationRevision pendingRevision in pendingRevisions)
            {
                pendingRevision.UpdateReviewer(immediateSupervisor);
            }
        }
    }
}