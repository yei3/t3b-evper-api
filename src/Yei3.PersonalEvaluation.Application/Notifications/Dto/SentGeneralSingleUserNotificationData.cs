using Abp.Notifications;

public class SentGeneralUserNotificationData : NotificationData
{
    public string SenderUserName { get; set; }

    public string GeneralMessage { get; set; }

    public SentGeneralUserNotificationData(string senderUserName, string generalMessage)
    {
        SenderUserName = senderUserName;
        GeneralMessage = generalMessage;
    }
}