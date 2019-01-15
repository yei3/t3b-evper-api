using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using Abp;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Runtime.Session;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Sessions;
using Abp.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace Yei3.PersonalEvaluation.Notifications
{
    public class NotificationsAppService : ApplicationService, INotificationsAppService
    {
        private readonly INotificationPublisher _notiticationPublisher;

        private readonly IUserNotificationManager  _userNotificationManager;

        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly ISessionAppService _sessionAppService;
        private readonly UserManager UserManager;

        private readonly IRepository<Abp.Organizations.OrganizationUnit, long> OrganizationUnitRepository;

         public NotificationsAppService(INotificationSubscriptionManager notificationSubscriptionManager, UserManager userManager, INotificationPublisher notiticationPublisher, IUserNotificationManager  userNotificationManager, IRepository<Abp.Organizations.OrganizationUnit, long> organizationUnitRepository)
        {
            _notificationSubscriptionManager = notificationSubscriptionManager;
            UserManager = userManager;
            _notiticationPublisher = notiticationPublisher;
            _userNotificationManager = userNotificationManager;
            OrganizationUnitRepository = organizationUnitRepository;
        }

        public async Task<List<UserNotification>> getAll()
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            return await _userNotificationManager.GetUserNotificationsAsync(new Abp.UserIdentifier(administratorUser.TenantId, administratorUser.Id), 0, 0, 100);
        }

        public async Task<int> getNotifCount()
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            return await _userNotificationManager.GetUserNotificationCountAsync(new Abp.UserIdentifier(administratorUser.TenantId, administratorUser.Id),0);
        }

        public async Task Publish_SentGeneralUserNotification(string senderUserName, string generalMessage)
        {
            User administratorUser = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
            UserIdentifier targetUserId = new UserIdentifier(administratorUser.TenantId, administratorUser.Id);
            await _notiticationPublisher.PublishAsync("GeneralNotification", new SentGeneralUserNotificationData(senderUserName, generalMessage), userIds: new[] { targetUserId });
        }

        public async Task Publish_SentGeneralMultipleUserNotification(CreateNotificationDto input)
        {
            if(string.IsNullOrEmpty(input.GeneralMessage)){
                throw new UserFriendlyException($"Por favor ingrese un mensaje.");
            }
            if(input.JobDescriptions.IsNullOrEmpty<string>() && input.UserIds.IsNullOrEmpty<long>() && input.OrganizationUnitIds.IsNullOrEmpty<long>()){
                throw new UserFriendlyException($"Debe seleccionar algún destinatario para el mensaje.");
            }
            List<User> users = await UserManager
                .Users
                .Where(user => input.UserIds.Contains(user.Id) ) 
                .ToListAsync();

            foreach (long inputOrganizationUnitId in input.OrganizationUnitIds)
            {
                Abp.Organizations.OrganizationUnit currentOrganizationUnit = await
                    OrganizationUnitRepository.FirstOrDefaultAsync(inputOrganizationUnitId);

                List<User> allUsers = await UserManager.GetUsersInOrganizationUnit(currentOrganizationUnit, true);

                users.AddRange(
                    allUsers
                        /* .Where(user => UserManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Collaborator).GetAwaiter().GetResult()
                        )*/
                    );
            }

            if(!input.JobDescriptions.IsNullOrEmpty<string>()){
                List<User> usersJob = await UserManager
                .Users
                .Where(user => input.JobDescriptions.Contains(user.JobDescription) ) 
                .ToListAsync();

                users.AddRange(
                    usersJob
                );

            }

            await _notiticationPublisher.PublishAsync("GeneralNotification", 
                        new SentGeneralUserNotificationData(input.SenderName, input.GeneralMessage), 
                        userIds: users.Select(user => new UserIdentifier(user.TenantId, user.Id)).ToArray());
        }

    }
}