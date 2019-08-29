using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Users.Dto
{
    public class UserFullNameDto : EntityDto<long>
    {
        public string FullName { get; set; }
    }
}