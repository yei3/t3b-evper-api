using System.Collections.Generic;

namespace Yei3.PersonalEvaluation.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
