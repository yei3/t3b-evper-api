using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Yei3.PersonalEvaluation.ExcelExport;

namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    [AutoMapFrom(typeof(FileValueObject))]
    public class FileDto
    {
        [Required]
        public string FileName { get; set; }

        public string FileType { get; set; }

        [Required]
        public string FileToken { get; set; }

        public FileDto()
        {
            
        }

        public FileDto(string fileName, string fileType)
        {
            FileName = fileName;
            FileType = fileType;
            FileToken = Guid.NewGuid().ToString("N");
        }
    }
}