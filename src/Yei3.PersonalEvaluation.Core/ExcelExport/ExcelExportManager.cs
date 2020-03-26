using System;
using System.Collections.Generic;
using Abp.Collections.Extensions;
using Abp.Domain.Services;
using Abp.Runtime.Caching;
using OfficeOpenXml;
using Yei3.PersonalEvaluation.Net.MimeTypes;

namespace Yei3.PersonalEvaluation.ExcelExport
{
    public class ExcelExportManager : DomainService, IExcelExportManager
    {
        private readonly ICacheManager CacheManager;

        public ExcelExportManager(ICacheManager cacheManager)
        {
            CacheManager = cacheManager;
        }

        public void AddExcelObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        public FileValueObject CreateExcelPackage(string cacheEntryName, string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileValueObject(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                SaveFileToCache(cacheEntryName, excelPackage, file);
            }

            return file;
        }

        protected void SaveFileToCache(string cacheEntryName, ExcelPackage excelPackage, FileValueObject file, int cacheLifeInMinutes = 1)
        {
            CacheManager.GetCache(cacheEntryName).Set(file.FileToken, excelPackage.GetAsByteArray(), new TimeSpan(0, 0, cacheLifeInMinutes, 0));
        }
    }
}