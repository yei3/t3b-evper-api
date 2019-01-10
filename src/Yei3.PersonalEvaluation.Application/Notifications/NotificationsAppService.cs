using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Abp.Application.Services;
using Abp.Notifications;
using Abp.Runtime.Session;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Notifications;
using Yei3.PersonalEvaluation.Sessions;

namespace Yei3.PersonalEvaluation.Notifications
{
    public class NotificationsAppService : ApplicationService, INotificationsAppService
    {
        private readonly INotificationPublisher _notiticationPublisher;

        private readonly IUserNotificationManager  _userNotificationManager;

        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly ISessionAppService _sessionAppService;
        private readonly UserManager UserManager;

         public NotificationsAppService(INotificationSubscriptionManager notificationSubscriptionManager, UserManager userManager, INotificationPublisher notiticationPublisher, IUserNotificationManager  userNotificationManager)
        {
            _notificationSubscriptionManager = notificationSubscriptionManager;
            UserManager = userManager;
            _notiticationPublisher = notiticationPublisher;
            _userNotificationManager = userNotificationManager;
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

    }
}