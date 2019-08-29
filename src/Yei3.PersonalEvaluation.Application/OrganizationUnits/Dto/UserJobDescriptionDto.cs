using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Application.OrganizationUnits.Dto
{
    [AutoMap(typeof(User))]
    public class UserJobDescriptionDto : EntityDto<long>
    {
        public string JobDescription { get; set; }
        public List<long> AreaIds { get; set; }
    }
}