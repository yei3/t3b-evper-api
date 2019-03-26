using Microsoft.AspNetCore.Antiforgery;
using Yei3.PersonalEvaluation.Controllers;

namespace Yei3.PersonalEvaluation.Web.Host.Controllers
{
    public class AntiForgeryController : PersonalEvaluationControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
