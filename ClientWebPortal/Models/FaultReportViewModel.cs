using System.ComponentModel.DataAnnotations;

namespace ClientWebPortal.Models
{
    public class FaultReportViewModel
    {
        [Required]
        [Display(Name = "Description")]
        public required string Description { get; set; }

        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Please enter a valid email address.")]
        [Required]
        [Display(Name = "Email address")]
        public required string Email { get; set; }
        public DateTime ReportedAt { get; set; }
        public string Status { get; set; } = "New";
        public Guid Id { get; set; }
        public AddressViewModel Address { get; set; }
    }
}

