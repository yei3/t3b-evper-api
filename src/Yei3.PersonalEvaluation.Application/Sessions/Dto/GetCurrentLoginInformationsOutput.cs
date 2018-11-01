using System.Collections.Generic;

namespace Yei3.PersonalEvaluation.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutput
    {
        public ApplicationInfoDto Application { get; set; }

        public UserLoginInfoDto User { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }

        public List<string> Roles {get; set;}
    }
}
