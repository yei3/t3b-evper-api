using Abp;
using Abp.Domain.Uow;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yei3.PersonalEvaluation.Authorization.Roles;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.DataAdquisition
{
    class Program
    {
        static string filePath;

        static void Main(string[] args)
        {

            CheckParams(args);

            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Hoja1"];
                int rowCount = worksheet.Dimension.Rows;

                using (var bootstrapper = AbpBootstrapper.Create<PersonalEvaluationDataAdquisitionModule>())
                {
                    bootstrapper.Initialize();

                    UserRegistrationManager userRegistrationManager = bootstrapper.IocManager.Resolve<UserRegistrationManager>();

                    for (int row = 3; row < rowCount; row++) // start in row 3 cause data starts there, any template change can break this, is better if we provide the template
                    {
                        try
                         {
                            User currentUser = userRegistrationManager.ImportUser(
                                employeeNumber: worksheet.Cells[row, 1].Value.ToString(),
                                status: worksheet.Cells[row, 2].Value.ToString() == AppConst.IsActiveImportValue,
                                firstLastName: worksheet.Cells[row, 3].Value.ToString(),
                                secondLastName: worksheet.Cells[row, 4].Value.ToString(),
                                name: worksheet.Cells[row, 5].Value.ToString(),
                                jobDescription: worksheet.Cells[row, 6].Value.ToString(),
                                area: worksheet.Cells[row, 7].Value.ToString(),
                                region: worksheet.Cells[row, 8].Value.ToString(),
                                immediateSupervisor: worksheet.Cells[row, 9].Value.ToString(),
                                socialReason: worksheet.Cells[row, 10].Value.ToString(),
                                isSupervisor: worksheet.Cells[row, 11].Value != null && worksheet.Cells[row, 11].Value.ToString().Contains('X'),
                                isManager: worksheet.Cells[row, 12].Value != null && worksheet.Cells[row, 12].Value.ToString().Contains('X'),
                                entryDate: worksheet.Cells[row, 13].Value.ToString(),
                                reassignDate: worksheet.Cells[row, 14].Value == null
                                    ? null
                                    : worksheet.Cells[row, 14].Value.ToString(),
                                birthDate: worksheet.Cells[row, 15].Value.ToString(),
                                scholarship: worksheet.Cells[row, 16].Value.ToString(),
                                email: worksheet.Cells[row, 17].Value.ToString()
                            );
                        }
                        catch (Exception)
                        {
                            System.Console.WriteLine($"Usuario {worksheet.Cells[row, 3].Value} fue agregado anteriormente.");
                        }
                    }
                }
            }
        }

        static void CheckParams(string[] args)
        {
            // param validation
            try
            {
                filePath = args[0];
            }
            catch (IndexOutOfRangeException)
            {
                filePath = "C:\\Users\\hackergateII\\Downloads\\18-09-03 Base de datos - RRHH HPA.xlsx";
            }
        }
    }
}
