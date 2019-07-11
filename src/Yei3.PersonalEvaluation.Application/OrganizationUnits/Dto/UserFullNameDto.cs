using System;
using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Application.OrganizationUnits.Dto
{
    public class UserFullNameDto : EntityDto<long>
    {
        public string FullName { get; set; }
    }
}