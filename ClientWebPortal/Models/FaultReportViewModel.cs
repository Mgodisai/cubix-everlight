using ClientWebPortal.Resources;
using System.ComponentModel.DataAnnotations;

namespace ClientWebPortal.Models
{
    public class FaultReportViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.DescriptionRequired))]
        public required string Description { get; set; }

        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", 
            ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.EmailFormatInvalid))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.EmailRequired))]
        public required string Email { get; set; }
        public DateTime ReportedAt { get; set; }
        public string Status { get; set; } = "New";
        public Guid Id { get; set; }
        public AddressViewModel? Address { get; set; }
    }
}

