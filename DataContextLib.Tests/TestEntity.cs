using DataContextLib.Models;

namespace DataContextLib.Tests;

public partial class DataRepositoryTests
{
    public class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid? AddressId { get; set; }
        public Address? Address { get; set; }
    }
}