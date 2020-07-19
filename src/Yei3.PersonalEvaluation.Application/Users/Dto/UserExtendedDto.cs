using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserExtendedDto : EntityDto<long>
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
        public bool IsMale { get; set; }
        public bool IsActive { get; set; } = true;
        public string Area { get; set; }
        public string AreaCode { get; set; }
        public string Region { get; set; }
        public string RegionCode { get; set; }
        public string ImmediateSupervisor { get; set; }
        public string SocialReason { get; set; }
        public string EntryDate { get; set; }
        public string ReassignDate { get; set; }
        public string BirthDate { get; set; }
        public string Scholarship { get; set; }
        public string JobDescription { get; set; }
        public string[] Roles { get; set; }
    }
}
