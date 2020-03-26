using System;
using System.Collections.Generic;
using Abp.Domain.Services;
using OfficeOpenXml;

namespace Yei3.PersonalEvaluation.ExcelExport
{
    public interface IExcelExportManager : IDomainService
    {
         void AddExcelObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors);
         FileValueObject CreateExcelPackage(string cacheEntryName, string fileName, Action<ExcelPackage> creator);
    }
}