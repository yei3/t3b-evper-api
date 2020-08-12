using System;
using System.IO;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Controllers;
using Yei3.PersonalEvaluation.Core.Authorization.Users;
using Yei3.PersonalEvaluation.Core.Authorization.Users.BackgroundJob;

namespace Yei3.PersonalEvaluation.Web.Host.Controllers
{
    [Route("api/[controller]")]
    public class ImportUsersController : PersonalEvaluationControllerBase
    {
        private readonly UserManager _userManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ImportUsersController(UserManager userManager, UserRegistrationManager userRegistrationManager, IBackgroundJobManager backgroundJobManager)
        {
            _userManager = userManager;
            _userRegistrationManager = userRegistrationManager;
            _backgroundJobManager = backgroundJobManager;
        }

        [HttpPost]
        public async Task ImportUsersAction(string emailAddress, [FromForm]IFormFile file)
        {
            string filePath = Path.GetTempFileName();
            
            // ImportUserSummaryModel importUserSummaryModel = new ImportUserSummaryModel("./View/ImportedUserSummaryView.cshtml", emailAddress);

            User administratorUser = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());

            if (!await _userManager.IsInRoleAsync(administratorUser, StaticRoleNames.Tenants.Administrator))
            {
                throw new UserFriendlyException(401, $"Usuario {administratorUser.FullName} no es un Administrador.");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                int rowCount = worksheet.Dimension.Rows;

                if (rowCount < 2)
                {
                    throw new UserFriendlyException(400, "Sin registros por procesar. Seleccione un archivo con mil registros máximo.");
                }
                if (rowCount > 1000)
                {
                    throw new UserFriendlyException(400, "Límite de registros por cargar/modificar excedido. Seleccione un archivo con mil registros máximo.");
                }

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

                        // importUserSummaryModel.ImportedUserDictionary.Add(currentUser.EmployeeNumber, currentUser.FullName);
                        // importUserSummaryModel.IncrementImportedUser();
                    }
                    catch (Exception e)
                    {
                        // importUserSummaryModel.IncrementNotImportedUser();
                        Logger.Info(e.Message);
                        Logger.Info($"Usuario {worksheet.Cells[row, 3].Value} fue agregado anteriormente.");
                    }
                }

                // await _backgroundJobManager.EnqueueAsync<SendImportUserReportBackgroundJob, ImportUserSummaryModel>(importUserSummaryModel);

            }
        }
    }
}