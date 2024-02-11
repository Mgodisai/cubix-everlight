namespace DataContextLib.Models;
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

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (Address)obj;
        return PostalCode == other.PostalCode &&
               City == other.City &&
               Street == other.Street &&
               HouseNumber == other.HouseNumber;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PostalCode, City, Street, HouseNumber);
    }
}