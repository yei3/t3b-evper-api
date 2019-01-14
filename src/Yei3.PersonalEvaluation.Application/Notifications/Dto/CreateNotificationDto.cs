using System.Collections.Generic;
using Abp.Application.Services.Dto;

public class CreateNotificationDto : EntityDto<long>
{
    public string SenderName { get; set; }

    public string GeneralMessage { get; set; }

    public ICollection<long> OrganizationUnitIds { get; set; }
    public ICollection<string> JobDescriptions { get; set; }

    public ICollection<long> UserIds { get; set; }

}