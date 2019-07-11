using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Application.OrganizationUnits.Dto
{
    public class UserJobDescriptionDto : EntityDto<long>
    {
        public string JobDescription { get; set; }
    }
}