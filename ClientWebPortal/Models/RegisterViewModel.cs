using ClientWebPortal.Resources;
using System.ComponentModel.DataAnnotations;

namespace ClientWebPortal.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.EmailRequired))]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.PasswordRequired))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages),
            ErrorMessageResourceName = nameof(ValidationMessages.PasswordLengthError), MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages),
           ErrorMessageResourceName = nameof(ValidationMessages.ConfirmPasswordRequired))]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(ValidationMessages),
            ErrorMessageResourceName = nameof(ValidationMessages.PasswordMatchError))]
        public string ConfirmPassword { get; set; }
    }
}
