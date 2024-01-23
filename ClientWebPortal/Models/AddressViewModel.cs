using ClientWebPortal.Resources;
using System.ComponentModel.DataAnnotations;

namespace ClientWebPortal.Models
{
    public class AddressViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.PostalCodeRequired))]
        [RegularExpression(@"^\d{4}$", ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.PostalCodeFormatInvalid))]
        public string? PostalCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.CityRequired))]
        public string? City { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.StreetRequired))]
        public string? Street { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), 
            ErrorMessageResourceName = nameof(ValidationMessages.HouseNumberRequired))]
        public string? HouseNumber { get; set; }

        public override string? ToString()
        {
            return $"{PostalCode} {City}, {Street} {HouseNumber}";
        }
    }
}
