using Abp.AutoMapper;
using Yei3.PersonalEvaluation.Authentication.External;

namespace Yei3.PersonalEvaluation.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
