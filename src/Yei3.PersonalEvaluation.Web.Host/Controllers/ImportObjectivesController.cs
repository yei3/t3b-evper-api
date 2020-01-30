using Abp.Application.Services;
using Abp.BackgroundJobs;
using Abp.UI;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;
using Yei3.PersonalEvaluation.Core.Evaluations.SalesObjectives;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Yei3.PersonalEvaluation.Web.Host.Controllers
{
    [Route("api/[controller]")]
    public class ImportObjectivesController : ApplicationService
    {
        private readonly UserManager _userManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly SalesObjectivesManager _salesObjectivesManager;

        public ImportObjectivesController(UserManager userManager, IBackgroundJobManager backgroundJobManager, SalesObjectivesManager salesObjectivesManager)
        {
            _userManager = userManager;
            _backgroundJobManager = backgroundJobManager;
            _salesObjectivesManager = salesObjectivesManager;
        }

        [HttpPost]
        public async Task ImportObjectivesAction(string emailAddress, [FromForm]IFormFile file)
        {
            string filePath = Path.GetTempFileName();

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

                var importGTObjectivesTask = ImportGTObjectivesAsync(package.Workbook.Worksheets["Indicadores GT"]);
                var importGDObjectivesTask = ImportGDObjectivesAsync(package.Workbook.Worksheets["Indicadores GD"]);
                var importGZObjectivesTask = ImportGZObjectivesAsync(package.Workbook.Worksheets["Indicadores GZ"]);

                await importGTObjectivesTask;
                await importGDObjectivesTask;
                await importGZObjectivesTask;
            }
        }

        internal async Task ImportGTObjectivesAsync(ExcelWorksheet worksheet)
        {
            int rowCount = worksheet.Dimension.Rows;
                
            //! start in row 3 cause data starts there, any template change can break this.
            for (int row = 3; row <= rowCount; row++)
            {
                try
                {
                    await _salesObjectivesManager.ImportGTSalesObjectivesAsync(
                        worksheet.Cells[row, 1].Value.ToString(),
                        parseToLong(worksheet.Cells[row, 6].Value.ToString()),
                        parseToLong(worksheet.Cells[row, 7].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 8].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 9].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 10].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 11].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 12].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 13].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 14].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 15].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 16].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 17].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 18].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 19].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 20].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 21].Value.ToString()),
                        worksheet.Cells[row, 22].Value.ToString(),
                        worksheet.Cells[row, 23].Value.ToString(),
                        parseToDecimal(worksheet.Cells[row, 24].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 25].Value.ToString())
                    );

                }
                catch (Exception e)
                {
                    Logger.Info(e.Message);
                    throw new UserFriendlyException(400, $"Falló la Carga de Objectivos de Ventas para GT. Error en la fila {row}");
                }
            }
        }

        internal async Task ImportGDObjectivesAsync(ExcelWorksheet worksheet)
        {
            int rowCount = worksheet.Dimension.Rows;
                
            //! start in row 3 cause data starts there, any template change can break this.
            for (int row = 3; row <= rowCount; row++)
            {
                try
                {
                    await _salesObjectivesManager.ImportGDSalesObjectivesAsync(
                        worksheet.Cells[row, 1].Value.ToString(),
                        parseToLong(worksheet.Cells[row, 6].Value.ToString()),
                        parseToLong(worksheet.Cells[row, 7].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 8].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 9].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 10].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 11].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 12].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 13].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 14].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 15].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 16].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 17].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 18].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 19].Value.ToString()),
                        worksheet.Cells[row, 20].Value.ToString(),
                        worksheet.Cells[row, 21].Value.ToString(),
                        parseToDecimal(worksheet.Cells[row, 22].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 23].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 24].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 25].Value.ToString())
                    );
                }
                catch (Exception e)
                {
                    Logger.Info(e.Message);
                    throw new UserFriendlyException(400, $"Falló la Carga de Objectivos de Ventas para GD. Error en la fila {row}");
                }
            }
        }

        internal async Task ImportGZObjectivesAsync(ExcelWorksheet worksheet)
        {
            int rowCount = worksheet.Dimension.Rows;
                
            //! start in row 3 cause data starts there, any template change can break this.
            for (int row = 3; row <= rowCount; row++)
            {
                try
                {
                    await _salesObjectivesManager.ImportGZSalesObjectivesAsync(
                        worksheet.Cells[row, 1].Value.ToString(),
                        parseToLong(worksheet.Cells[row, 6].Value.ToString()),
                        parseToLong(worksheet.Cells[row, 7].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 8].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 9].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 10].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 11].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 12].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 13].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 14].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 15].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 16].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 17].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 18].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 19].Value.ToString()),
                        worksheet.Cells[row, 20].Value.ToString(),
                        worksheet.Cells[row, 21].Value.ToString(),
                        parseToDecimal(worksheet.Cells[row, 22].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 23].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 24].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 25].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 26].Value.ToString()),
                        parseToDecimal(worksheet.Cells[row, 27].Value.ToString())
                    );
                }
                catch (Exception e)
                {
                    Logger.Info(e.Message);
                    throw new UserFriendlyException(400, $"Falló la Carga de Objectivos de Ventas para GZ. Error en la fila {row}");
                }
            }
        }

        internal static long parseToLong(string number)
        {
            try
            {
                if (long.TryParse(number, out long x))
                    return x;
                else
                    return 0;
            }
            catch (System.Exception)
            {
                throw new UserFriendlyException(400, $"Import Objectives Parse failed");
            }
            
        }

        internal static decimal parseToDecimal(string number)
        {
            try
            {
                if (decimal.TryParse(number, out decimal x))
                    return x;
                else
                    return 0;
            }
            catch (System.Exception)
            {
                throw new UserFriendlyException(400, $"Import Objectives Parse failed");
            }
        }
    }
}