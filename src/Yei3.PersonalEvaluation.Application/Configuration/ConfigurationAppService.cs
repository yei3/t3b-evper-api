using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Yei3.PersonalEvaluation.Configuration.Dto;

namespace Yei3.PersonalEvaluation.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : PersonalEvaluationAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
