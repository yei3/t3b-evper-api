using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.OrganizationUnits.Dto
{
    [AutoMap(typeof(User))]
    public class OrganizationUnitUserDto : EntityDto<long>
    {
        public string ImmediateSupervisor { get; set; }
        public string JobDescription { get; set; }

        public string FullName { get; set; }
        public string UserName { get; set; }
    }
}