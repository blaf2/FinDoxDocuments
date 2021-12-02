using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FinDoxDocumentsAPI.Models
{
    public class PasswordAttribute : ValidationAttribute
    {
        private const string LengthMessage = "password must be at least 8 characters in length";
        private const string NumberMessage = "password must contain a number";
        private const string LowerCaseMessage = "password must contain a lower case letter";
        private const string UpperCaseMessage = "password must contain an upper case letter";
        private const string SpecialCharMessage = "password must contain a special character";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value.ToString();

            if (password.Length < 8)
                return new ValidationResult(LengthMessage);

            if (!Regex.IsMatch(password, @"[0-9]+"))
                return new ValidationResult(NumberMessage);

            if (!Regex.IsMatch(password, @"[a-z]+"))
                return new ValidationResult(LowerCaseMessage);

            if (!Regex.IsMatch(password, @"[A-Z]+"))
                return new ValidationResult(UpperCaseMessage);

            if (!Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,(,)]+"))
                return new ValidationResult(SpecialCharMessage);

            return ValidationResult.Success;
        }
    }
}
