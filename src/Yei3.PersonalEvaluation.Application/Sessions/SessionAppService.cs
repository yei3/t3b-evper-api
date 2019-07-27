using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Sessions.Dto;

namespace Yei3.PersonalEvaluation.Sessions
{
    public class SessionAppService : PersonalEvaluationAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {

                User user = await GetCurrentUserAsync();

                output.User = ObjectMapper.Map<UserLoginInfoDto>(user);
                output.Roles = ObjectMapper.Map<List<string>>(await this.UserManager.GetRolesAsync(user));
            }

            return output;
        }
    }
}
