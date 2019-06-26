using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Yei3.PersonalEvaluation.Roles.Dto;
using Yei3.PersonalEvaluation.Users.Dto;

namespace Yei3.PersonalEvaluation.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
        Task ChangeLanguage(ChangeUserLanguageDto input);
        Task UpdateScholarshipAndEmailAddress(UpdateUserDto updateUser);

        Task RecoverPassword(RecoverPasswordDto recoverPassword);
        Task<bool> IsUserSalesMan();

        Task<ICollection<string>> GetAllJobDescriptions();
    }
}
