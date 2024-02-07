using DataContextLib.Models;
using DataContextLib.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DataContextLib.Tests;

[TestFixture]
public class DataRepositoryTests
{
    private DbContextOptions<DbContext> _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        using (var context = new TestDbContext(_options))
        {
            // Arrange
            var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);
            context.Set<Address>().Add(new Address { Id = Guid.NewGuid() });
            context.Set<Address>().Add(new Address { Id = Guid.NewGuid() });
            await context.SaveChangesAsync();

            // Act
            var items = await repository.GetAllAsync();

            // Assert
            items.Should().HaveCount(2, because: "Two entities have been added to the database");
        }
    }

    [Test]
    public async Task GetByIdAsync_ReturnsItem_IfExists()
    {
        var testId = Guid.NewGuid();
        using (var context = new TestDbContext(_options))
        {
            // Arrange
            var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);
            context.Set<Address>().Add(new Address { Id = testId });
            await context.SaveChangesAsync();

            // Act
            var item = await repository.GetByIdAsync(testId);

            // Assert
            item.Should().NotBeNull(because: "the entity with the specified ID exists in the database");
            item.Id.Should().Be(testId, because: "the entity's ID should match the requested ID");
        }
    }

    [Test]
    public async Task InsertAsync_AddsNewItem()
    {
        using (var context = new TestDbContext(_options))
        {
            // Arrange
            var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);
            var newItem = new Address { Id = Guid.NewGuid() };

            // Act
            var result = await repository.InsertAsync(newItem);
            var addedItem = await repository.GetByIdAsync(newItem.Id);

            // Assert
            result.Should().BeTrue(because: "the entity should be successfully inserted");
            addedItem.Should().NotBeNull(because: "the inserted entity should be retrievable from the database");
        }
    }

    [Test]
    public async Task DeleteAsync_RemovesItem_IfExists()
    {
        using (var context = new TestDbContext(_options))
        {
            // Arrange
            var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);
            var testEntity = new Address { Id = Guid.NewGuid() };
            await context.Set<Address>().AddAsync(testEntity);
            await context.SaveChangesAsync();

            // Act
            var deleteResult = await repository.DeleteAsync(testEntity.Id);
            await context.SaveChangesAsync();
            var deletedEntity = await repository.GetByIdAsync(testEntity.Id);

            // Assert
            deleteResult.Should().BeTrue(because: "the entity should be successfully deleted from the database");
            deletedEntity.Should().BeNull(because: "the deleted entity should no longer exist in the database");
        }
    }

    [Test]
    public async Task UpdateAsync_UpdatesItem_IfExists()
    {
        using (var context = new TestDbContext(_options))
        {
            // Arrange
            var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);
            Guid guid = Guid.NewGuid();
            var testEntity = new Address { Id = guid, Street = "Street", City = "City" };
            await context.Addresses.AddAsync(testEntity);
            await context.SaveChangesAsync();

            // Prepare the entity for update
            testEntity.Street = "New Street";
            testEntity.City = "New City";

            // Act
            var updateResult = await repository.UpdateAsync(testEntity);
            var updatedEntity = await context.Addresses.FindAsync(guid);

            // Assert
            updateResult.Should().BeTrue(because: "the entity should be successfully updated in the database");
            updatedEntity.Should().NotBeNull();
            updatedEntity.Street.Should().Be("New Street", because: "the street name should be updated");
            updatedEntity.City.Should().Be("New City", because: "the city name should be updated");
            updatedEntity.Id.Should().Be(guid, because: "the guid should not be updated");
        }
    }


    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<DbContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
    }
}