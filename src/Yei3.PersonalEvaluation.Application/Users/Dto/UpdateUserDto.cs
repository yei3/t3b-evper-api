using Abp.Application.Services.Dto;
using Abp.Domain.Entities;

namespace Yei3.PersonalEvaluation.Users.Dto
{
    public class UpdateUserDto
    {
        public string EmailAddress { get; set; }
        public string Scholarship { get; set; }
    }
}