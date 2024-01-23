using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClientWebPortal.Models
{
    public class EmployeeViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Display name is required")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public Guid PositionId { get; set; }
        public string? PositionName { get; set; }
        public List<SelectListItem>? Positions { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
