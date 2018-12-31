using System.Threading.Tasks;
using Abp.Notifications;
using Yei3.PersonalEvaluation.Notifications;

namespace Yei3.PersonalEvaluation.Notifications
{
    public class NotificationsAppService : INotificationsAppService
    {
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;

         public NotificationsAppService(INotificationSubscriptionManager notificationSubscriptionManager)
        {
            _notificationSubscriptionManager = notificationSubscriptionManager;
        }

        //Subscribe to a general notification
        public async Task Subscribe_SentFriendshipRequest(int? tenantId, long userId)
        {
            await _notificationSubscriptionManager.SubscribeAsync(new Abp.UserIdentifier(tenantId, userId), "SentFriendshipRequest");    
        }

    }
}