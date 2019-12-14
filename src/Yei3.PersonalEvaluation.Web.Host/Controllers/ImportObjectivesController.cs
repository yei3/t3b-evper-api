using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Abp.BackgroundJobs;
using OfficeOpenXml;
using Yei3.PersonalEvaluation.Controllers;
using Yei3.PersonalEvaluation.Core.Evaluations.SalesObjectives;

namespace Yei3.PersonalEvaluation.Web.Host.Controllers
{
    [Route("api/[controller]")]
    public class ImportObjectivesController : PersonalEvaluationControllerBase
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly SalesObjectivesManager _salesObjectivesManager;

        public ImportObjectivesController(IBackgroundJobManager backgroundJobManager, SalesObjectivesManager salesObjectivesManager)
        {
            _backgroundJobManager = backgroundJobManager;
            _salesObjectivesManager = salesObjectivesManager;
        }

        [HttpPost]
        public async Task<IActionResult> ImportObjectivesAction(string emailAddress, [FromForm]IFormFile file)
        {
            string filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Indicadores GT"];

                int rowCount = worksheet.Dimension.Rows;
                
                //! start in row 4 cause data starts there, any template change can break this.
                for (int row = 4; row <= rowCount; row++)
                {
                    try
                    {
                        await _salesObjectivesManager.ImportGTObjectivesAsync(
                            worksheet.Cells[row, 1].Value.ToString(),
                            worksheet.Cells[row, 6].Value.ToString(),
                            worksheet.Cells[row, 7].Value.ToString(),
                            worksheet.Cells[row, 8].Value.ToString(),
                            worksheet.Cells[row, 9].Value.ToString(),
                            worksheet.Cells[row, 10].Value.ToString(),
                            worksheet.Cells[row, 11].Value.ToString(),
                            worksheet.Cells[row, 12].Value.ToString(),
                            worksheet.Cells[row, 13].Value.ToString(),
                            worksheet.Cells[row, 14].Value.ToString(),
                            worksheet.Cells[row, 15].Value.ToString(),
                            worksheet.Cells[row, 16].Value.ToString(),
                            worksheet.Cells[row, 17].Value.ToString(),
                            worksheet.Cells[row, 18].Value.ToString(),
                            worksheet.Cells[row, 19].Value.ToString(),
                            worksheet.Cells[row, 20].Value.ToString(),
                            worksheet.Cells[row, 21].Value.ToString(),
                            worksheet.Cells[row, 22].Value.ToString(),
                            worksheet.Cells[row, 23].Value.ToString()
                        );

                    }
                    catch (Exception e)
                    {
                        // importUserSummaryModel.IncrementNotImportedUser();
                        Logger.Info(e.Message);
                    }
                }

                return Ok();
            }
        }
    }
}