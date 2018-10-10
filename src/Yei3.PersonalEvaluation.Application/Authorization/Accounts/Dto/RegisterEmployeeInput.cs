using Abp.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Application.Authorization.Accounts.Dto
{
    public class RegisterEmployeeInput : IValidatableObject
    {
        [Required]
        [StringLength(User.EmployeeNumberLength)]
        public string EmployeeNumber {get; set;}

        [Required]
        [DisableAuditing]
        public string Password { get; set; }
        [Required]
        [DisableAuditing]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(EmployeeNumber.Length != User.EmployeeNumberLength) {
                yield return new ValidationResult($"El numero de empleado {EmployeeNumber} no es correcto.");
            }

            if (Password != ConfirmPassword)
            {
                yield return new ValidationResult($"Passwords no coinciden.");
            }
        }
    }
}