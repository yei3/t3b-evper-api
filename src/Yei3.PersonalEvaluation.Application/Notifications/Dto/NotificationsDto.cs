namespace Yei3.PersonalEvaluation.OrganizationUnits.Dto
{
    using Abp.AutoMapper;
    using Abp.Domain.Entities;
    [AutoMap(typeof(Abp.Notifications.UserNotification))]
    public class NotificationsDto : Entity<long>
    {
        public string NotificationName { get; set; }
        public string DataTypeName { get; set; }
        public long? Data { get; set; }
        public int Severity { get; set; }
        public long? UserIds { get; set; }
        public long? TenantIds { get; set; }

    }
}