using Abp.Notifications;

public class SentReopenedNotificationData : NotificationData
{
    public long UserId { get; set; }

    public string ObjectiveName { get; set; }

    public SentReopenedNotificationData(long userId, string objectiveName)
    {
        UserId = userId;
        ObjectiveName = objectiveName;
    }
}