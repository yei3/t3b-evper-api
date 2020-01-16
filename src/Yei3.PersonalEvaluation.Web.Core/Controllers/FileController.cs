using Abp.Auditing;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Mvc;
using Yei3.PersonalEvaluation.Evaluations.Dto;

namespace Yei3.PersonalEvaluation.Controllers
{
    [Route("api/[controller]/[action]")]
    public class FileController : PersonalEvaluationControllerBase
    {
        private readonly ICacheManager CacheManager;

        public FileController(ICacheManager cacheManager)
        {
            CacheManager = cacheManager;
        }

        [DisableAuditing]
        [HttpGet]
        public ActionResult DownloadTempFile(FileDto file)
        {
            if (!(CacheManager.GetCache(AppConsts.TempEvaluationStatusesFileName).Get(file.FileToken, ep => ep) is byte[] fileBytes))
            {
                return NotFound("El archivo requerido no fue encontrado.");
            }

            return File(fileBytes, file.FileType, file.FileName);
        }
    }
}