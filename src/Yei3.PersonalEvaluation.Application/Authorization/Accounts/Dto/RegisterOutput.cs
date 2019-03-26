using System.Collections.Generic;

namespace Yei3.PersonalEvaluation.Authorization.Accounts.Dto
{
    public class RegisterOutput
    {
        public bool HasErrors { get; set; }
        public bool CanLogin { get; set; }
        public List<string> Errors { get; set; }
    }
}
