using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DataContextLib.Models;

namespace DataContextLib.Tests
{
    [TestFixture]
    public class DataDbContextTests
    {
        private DataDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new DataDbContext(options);

            _context.Addresses.Add(new Address { PostalCode = "7400" });
            _context?.SaveChanges();
        }

        [Test]
        public async Task Address_Add_NewAddressIsSaved()
        {
            // Arrange
            var address = new Address { PostalCode = "NewPostalCode" };

            // Act
            _context?.Addresses?.Add(address);
            await _context.SaveChangesAsync();

            // Assert
            var savedAddress = await _context.Addresses.FirstOrDefaultAsync(r => r.PostalCode == "NewPostalCode");
            savedAddress.Should().NotBeNull();
            savedAddress?.PostalCode.Should().Be("NewPostalCode", "because we set this postal code before");
        }

        [Test]
        public async Task Address_Update_ExistingAddressIsUpdated()
        {
            // Arrange
            var existingAddress = await _context.Addresses.FirstOrDefaultAsync();
            existingAddress.PostalCode = "UpdatedPostalCode";

            // Act
            _context.Addresses.Update(existingAddress);
            await _context.SaveChangesAsync();

            // Assert
            var updatedAddress = await _context.Addresses.FindAsync(existingAddress.Id);
            updatedAddress.Should().NotBeNull();
            updatedAddress.PostalCode.Should().Be("UpdatedPostalCode");
        }

        [Test]
        public async Task Address_Delete_ExistingAddressIsRemoved()
        {
            // Arrange
            var newAddress = new Address { PostalCode = "DeletePostalCode" };
            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync();

            // Act
            _context.Addresses.Remove(newAddress);
            await _context.SaveChangesAsync();

            // Assert
            var deletedAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.PostalCode == "DeletePostalCode");
            deletedAddress.Should().BeNull();
        }

        [Test]
        public async Task Address_GetAll_AddressesAreRetrieved()
        {
            // Arrange
            var newAddress = new Address { PostalCode = "DeletePostalCode" };
            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync();

            // Act
            var addresses = await _context.Addresses.ToListAsync();

            // Assert
            addresses.Count.Should().Be(2);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
