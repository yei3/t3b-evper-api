using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Controllers;
using Yei3.PersonalEvaluation.Core.Authorization.Users;
using Abp.BackgroundJobs;
using Yei3.PersonalEvaluation.Core.Authorization.Users.BackgroundJob;

namespace Yei3.PersonalEvaluation.Web.Host.Controllers
{
    [Route("api/[controller]")]
    public class ImportUsersController : PersonalEvaluationControllerBase
    {

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ImportUsersController(UserRegistrationManager userRegistrationManager, IBackgroundJobManager backgroundJobManager)
        {
            _userRegistrationManager = userRegistrationManager;
            _backgroundJobManager = backgroundJobManager;
        }

        [HttpPost]
        public async Task<IActionResult> ImportUsersAction(string emailAddress, [FromForm]IFormFile file)
        {
            string filePath = Path.GetTempFileName();
            
            ImportUserSummaryModel importUserSummaryModel = new ImportUserSummaryModel("./View/ImportedUserSummaryView.cshtml", emailAddress);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                int rowCount = worksheet.Dimension.Rows;

                //* start in row 2 cause data starts there, any template change can break this, is better if we provide the template
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        User currentUser = await _userRegistrationManager.ImportUserAsync(
                            employeeNumber: worksheet.Cells[row, 1].Value.ToString(),
                            status: worksheet.Cells[row, 2].Value.ToString() == "ACTIVO",
                            lastName: $"{worksheet.Cells[row, 3].Value.ToString()} {worksheet.Cells[row, 4].Value?.ToString()}",
                            name: worksheet.Cells[row, 5].Value.ToString(),
                            jobDescription: worksheet.Cells[row, 6].Value.ToString(),
                            area: worksheet.Cells[row, 7].Value.ToString(),
                            region: worksheet.Cells[row, 9].Value.ToString(),
                            immediateSupervisor: worksheet.Cells[row, 10].Value.ToString(),
                            socialReason: worksheet.Cells[row, 11].Value.ToString(),
                            isSupervisor: worksheet.Cells[row, 12].Value != null 
                                && worksheet.Cells[row, 12].Value.ToString().Contains('X', StringComparison.InvariantCultureIgnoreCase),
                            isManager: worksheet.Cells[row, 13].Value != null
                                && worksheet.Cells[row, 13].Value.ToString().Contains('X', StringComparison.InvariantCultureIgnoreCase),
                            entryDate: worksheet.Cells[row, 14].Value.ToString(),
                            reassignDate: worksheet.Cells[row, 15].Value == null
                                ? null
                                : worksheet.Cells[row, 15].Value.ToString(),
                            birthDate: worksheet.Cells[row, 16].Value?.ToString(),
                            scholarship: worksheet.Cells[row, 17].Value == null
                                ? null
                                : worksheet.Cells[row, 17].Value.ToString(),
                            email: worksheet.Cells[row, 18].Value == null
                                ? null
                                : worksheet.Cells[row, 18].Value.ToString(),
                            isMale: worksheet.Cells[row, 19].Value.ToString() == "MASCULINO",
                            isSalesArea: worksheet.Cells[row, 8].Value != null
                                && worksheet.Cells[row, 8].Value.ToString().Contains('X', StringComparison.InvariantCultureIgnoreCase)
                        );

                        importUserSummaryModel.ImportedUserDictionary.Add(currentUser.EmployeeNumber, currentUser.FullName);
                        importUserSummaryModel.IncrementImportedUser();
                    }
                    catch (Exception e)
                    {
                        importUserSummaryModel.IncrementNotImportedUser();
                        Logger.Info(e.Message);
                        Logger.Info($"Usuario {worksheet.Cells[row, 3].Value} fue agregado anteriormente.");
                    }
                }

                await _backgroundJobManager.EnqueueAsync<SendImportUserReportBackgroundJob, ImportUserSummaryModel>(importUserSummaryModel);

                return Ok();
            }
        }
    }
}