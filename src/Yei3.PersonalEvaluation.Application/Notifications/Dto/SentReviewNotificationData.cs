using Abp.Notifications;

public class SentReviewNotificationData : NotificationData
{
    public long EvaluationId { get; set; }

    public string DateReview { get; set; }

    public SentReviewNotificationData(long evaluationId, string dateReview)
    {
        EvaluationId = evaluationId;
        DateReview = dateReview;
    }
}