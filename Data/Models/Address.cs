namespace Data.Models
{
    public class Address : BaseEntity
    {
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }

        public override string? ToString()
        {
            return $"{PostalCode} {City}, {Street} {HouseNumber}";
        }
    }
}
