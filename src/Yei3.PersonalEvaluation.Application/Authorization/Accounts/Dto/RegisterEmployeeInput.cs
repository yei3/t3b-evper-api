using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Yei3.PersonalEvaluation.Authorization.Users;

namespace Yei3.PersonalEvaluation.Authorization.Accounts.Dto
{
    public class RegisterEmployeeInput : IValidatableObject
    {
        [Required]
        public string EmployeeNumber {get; set;}

        [Required, EmailAddress]
        public string Email {get; set;}

        [Required]
        [DisableAuditing]
        public string Password { get; set; }
        [Required]
        [DisableAuditing]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EmployeeNumber.Length != User.EmployeeNumberLength) {
                yield return new ValidationResult($"El numero de empleado {EmployeeNumber} no es correcto.");
            }

            if (Password != ConfirmPassword)
            {
                yield return new ValidationResult($"Passwords no coinciden.");
            }
        }
    }
}