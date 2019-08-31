using System.Collections.Generic;
using Abp.Configuration;

namespace Yei3.PersonalEvaluation.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                new SettingDefinition("SendGridKey.Key", "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", scopes: SettingScopes.Tenant, isVisibleToClients: true)
            };
        }
    }
}
