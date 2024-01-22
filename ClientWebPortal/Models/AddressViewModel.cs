using System.ComponentModel.DataAnnotations;

namespace ClientWebPortal.Models
{
    public class AddressViewModel
    {
        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Postal Code is Required")]
        [RegularExpression(@"^\d{4,5}$", ErrorMessage = "Please enter a valid Postal Code (4 or 5 digit format")]
        public string? PostalCode { get; set; }

        [Required]
        [Display(Name = "City")]
        public string? City { get; set; }

        [Required]
        [Display(Name = "Street")]
        public string? Street { get; set; }

        [Required]
        [Display(Name = "House Number")]
        public string? HouseNumber { get; set; }

        public override string? ToString()
        {
            return $"{PostalCode} {City}, {Street} {HouseNumber}";
        }
    }
}
