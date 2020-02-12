using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.OrganizationUnits.Dto;

namespace Yei3.PersonalEvaluation.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserExtendedDto : EntityDto<long>
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
        public bool IsMale { get; set; }
        public bool IsActive { get; set; }
        public bool IsManager { get; set; } = false;
        public bool IsSupervisor { get; set; } = false;
        public bool IsSalesArea { get; set; } = false;
        public string Area { get; set; }
        public string Region { get; set; }
        public string ImmediateSupervisor { get; set; }
        public string SocialReason { get; set; }
        public string EntryDate { get; set; }
        public string ReassignDate { get; set; }
        public string BirthDate { get; set; }
        public string Scholarship { get; set; }
        public string JobDescription { get; set; }
    }
}
