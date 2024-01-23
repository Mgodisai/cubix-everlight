using ClientWebPortal.Resources;
using System.ComponentModel.DataAnnotations;

namespace ClientWebPortal.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.EmailRequired))]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.PasswordRequired))]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

