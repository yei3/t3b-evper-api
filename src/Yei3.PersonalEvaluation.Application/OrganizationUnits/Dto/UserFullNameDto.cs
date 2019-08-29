using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Application.OrganizationUnits.Dto
{
    [AutoMap(typeof(User))]
    public class UserFullNameDto : EntityDto<long>
    {
        public string FullName { get; set; }
        public string JobDescription { get; set; }
    }
}