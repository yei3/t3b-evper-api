using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Yei3.PersonalEvaluation.Controllers
{
    [Route("api/[controller]")]
    public class FileUploadController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateMediaItem(string name, [FromForm]IFormFile file)
        {
            // Do something with the file
            return Ok();
        }
    }
}