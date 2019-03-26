using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Yei3.PersonalEvaluation.Controllers
{
    public abstract class PersonalEvaluationControllerBase: AbpController
    {
        protected PersonalEvaluationControllerBase()
        {
            LocalizationSourceName = PersonalEvaluationConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
