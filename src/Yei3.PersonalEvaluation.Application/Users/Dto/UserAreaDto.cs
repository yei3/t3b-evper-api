using Abp.Application.Services.Dto;

namespace Yei3.PersonalEvaluation.Users.Dto
{
    public class UserAreaDto : EntityDto<long>
    {
        public string FullName { get; set; }
        public string JobDescription { get; set; }
        public long AreaId { get; set; }
    }
}