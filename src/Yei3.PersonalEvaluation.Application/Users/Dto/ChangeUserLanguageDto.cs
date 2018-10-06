using System.ComponentModel.DataAnnotations;

namespace Yei3.PersonalEvaluation.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}