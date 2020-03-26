using System;
using System.IO;
using Abp.Domain.Values;
using Abp.Extensions;

namespace Yei3.PersonalEvaluation.ExcelExport
{
    public class FileValueObject : ValueObject<FileValueObject>
    {
        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileToken { get; set; }

        public FileValueObject(string fileName, string fileType)
        {
            if (fileName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException($"Invalid path \"{fileName}\" for: {nameof(fileName)}");
            }
            
            if (fileType.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(fileType));
            }

            FileName = fileName;
            FileType = fileType;
            FileToken = Guid.NewGuid().ToString("N");
        }
    }
}