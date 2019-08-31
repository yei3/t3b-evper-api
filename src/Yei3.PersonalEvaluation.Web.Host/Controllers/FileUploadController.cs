using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yei3.PersonalEvaluation.Controllers;

namespace Yei3.PersonalEvaluation.Web.Host.Controllers
{
    [Route("api/[controller]")]
    public class FileUploadController : PersonalEvaluationControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateMediaItem(string name, [FromForm]IFormFile file)
        {
            // Do something with the file
            return Ok();
        }
    }
}