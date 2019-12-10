using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Yei3.PersonalEvaluation.Controllers;
using Abp.BackgroundJobs;

namespace Yei3.PersonalEvaluation.Web.Host.Controllers
{
    [Route("api/[controller]")]
    public class ImportObjectivesController : PersonalEvaluationControllerBase
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ImportObjectivesController(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        [HttpPost]
        public async Task<IActionResult> ImportObjectivesAction(string emailAddress, [FromForm]IFormFile file)
        {
            return Ok();
        }
    }
}