using System;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Evaluations.EvaluationRevisions;

namespace Yei3.PersonalEvaluation.Core.Authorization.Users.EventHandlers
{
    public class UserHandler :
        IEventHandler<EntityDeletingEventData<User>>,
        IEventHandler<EntityChangedEventData<User>>,
        ITransientDependency
    {

        private readonly UserManager _userManager;
        private readonly IRepository<EvaluationRevision, long> _evaluationRevisionRepository;

        public UserHandler(
            UserManager userManager,
            IRepository<EvaluationRevision, long> evaluationRevisionRepository
        )
        {
            _userManager = userManager;
            _evaluationRevisionRepository = evaluationRevisionRepository;
        }

        [UnitOfWork]
        public void HandleEvent(EntityDeletingEventData<User> eventData)
        {
            UpdateSupervisorSubordinateRelation(eventData.Entity);
        }
        
        [UnitOfWork]
        public void HandleEvent(EntityChangedEventData<User> eventData)
        {
            if (eventData.Entity.IsActive)
            {
                return;
            }

            UpdateSupervisorSubordinateRelation(eventData.Entity);
        }

        private void UpdateSupervisorSubordinateRelation(User eventUser)
        {
            try
            {
                // find user immediate superior
                User immediateSupervisor = _userManager.Users.FirstOrDefault(user => user.JobDescription == eventUser.ImmediateSupervisor);
                // find subordinates
                IQueryable<User> subordinates = _userManager.Users
                    .Where(user => user.ImmediateSupervisor == eventUser.JobDescription);

                // update to new immediate supervisor
                foreach (User subordinate in subordinates)
                {
                    subordinate.ImmediateSupervisor = immediateSupervisor.JobDescription;
                }

                // find dandling revisions
                IQueryable<EvaluationRevision> pendingRevisions = _evaluationRevisionRepository
                    .GetAll()
                    .Where(evaluationRevision => evaluationRevision.ReviewerUserId == eventUser.Id);

                // update new reviewer
                foreach (EvaluationRevision pendingRevision in pendingRevisions)
                {
                    pendingRevision.UpdateReviewer(immediateSupervisor);
                }
            }
            catch (System.Exception)
            {
                // throw new Exception($"Usuario {eventUser.FullName} no tiene supervisor definido.");
                return;
            }
        }
    }
}