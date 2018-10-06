using System.ComponentModel.DataAnnotations;

namespace Yei3.PersonalEvaluation.Configuration.Dto
{
    public class ChangeUiThemeInput
    {
        [Required]
        [StringLength(32)]
        public string Theme { get; set; }
    }
}
