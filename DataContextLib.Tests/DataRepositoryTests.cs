using DataContextLib.Models;
using DataContextLib.Repository;
using DataContextLib.Specifications;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DataContextLib.Tests;

[TestFixture]
public partial class DataRepositoryTests
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
        using var context = new TestDbContext(_options);

        // Arrange
        var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);
        context.Set<Address>().Add(new Address { Id = Guid.NewGuid() });
        context.Set<Address>().Add(new Address { Id = Guid.NewGuid() });
        await context.SaveChangesAsync();

        // Act
        var items = await repository.GetAllAsync();

        // Assert
        items.Should().HaveCount(2);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsItem_IfExists()
    {
        var testId = Guid.NewGuid();
        using var context = new TestDbContext(_options);

        // Arrange
        var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);
        context.Set<Address>().Add(new Address { Id = testId });
        await context.SaveChangesAsync();

        // Act
        var item = await repository.GetByIdAsync(testId);

        // Assert
        item.Should().NotBeNull();
        item?.Id.Should().Be(testId);
    }

    [Test]
    public async Task InsertAsync_AddsNewItem()
    {
        using var context = new TestDbContext(_options);
        // Arrange
        var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);
        var newItem = new Address { Id = Guid.NewGuid() };

        // Act
        var result = await repository.InsertAsync(newItem);
        var addedItem = await repository.GetByIdAsync(newItem.Id);

        // Assert
        result.Should().BeTrue();
        addedItem.Should().NotBeNull();
    }

    [Test]
    public async Task DeleteAsync_RemovesItem_IfExists()
    {
        using var context = new TestDbContext(_options);

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
        deleteResult.Should().BeTrue();
        deletedEntity.Should().BeNull();
    }

    [Test]
    public async Task UpdateAsync_UpdatesItem_IfExists()
    {
        using var context = new TestDbContext(_options);

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
        updateResult.Should().BeTrue();
        updatedEntity.Should().NotBeNull();
        updatedEntity?.Street.Should().Be("New Street");
        updatedEntity?.City.Should().Be("New City");
        updatedEntity?.Id.Should().Be(guid);
    }

    [Test]
    public async Task FindWithSpecificationAsync_ReturnsFilteredItems()
    {
        using var context = new TestDbContext(_options);
        var repository = new DataRepository<Address>(context, NullLogger<DataRepository<Address>>.Instance);

        // Arrange
        string testPostalCode = "1234";
        var address1 = new Address { Id = Guid.NewGuid(), PostalCode = testPostalCode };
        var address2 = new Address { Id = Guid.NewGuid(), PostalCode = "9876" };
        context.Set<Address>().AddRange(address1, address2);
        await context.SaveChangesAsync();

        var spec = new AddressWithConditionSpecification(testPostalCode);

        // Act
        var items = await repository.FindWithSpecificationAsync(spec);

        // Assert
        items.Should().ContainSingle()
            .And.Contain(x => x.PostalCode == testPostalCode);
    }

    [Test]
    public async Task FindWithSpecificationAsync_IncludesAddress()
    {
        using var context = new TestDbContext(_options);
        var repository = new DataRepository<TestEntity>(context, NullLogger<DataRepository<TestEntity>>.Instance);

        // Arrange
        var address = new Address()
        {
            Id = Guid.NewGuid(),
            PostalCode = "postalcode"
        };
        context.Set<Address>().Add(address);
        await context.SaveChangesAsync();

        var testEntity = new TestEntity()
        {
            Name = "testEntity",
            Address = address
        };
        context.Set<TestEntity>().Add(testEntity);
        await context.SaveChangesAsync();

        var spec = new TestEntityWithAddressSpecification();

        // Act
        var entities = await repository.FindWithSpecificationAsync(spec);

        // Assert
        entities.Should().NotBeEmpty()
            .And.OnlyContain(entity => entity.Address != null);
    }



    private class TestDbContext(DbContextOptions<DbContext> options) : DbContext(options)
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<TestEntity> TestEntities { get; set; }
    }

    public class AddressWithConditionSpecification : BaseSpecification<Address>
    {
        public AddressWithConditionSpecification(string postalCode)
        {
            AddCriteria(x => x.PostalCode == postalCode);
        }
    }

    public class TestEntityWithAddressSpecification : BaseSpecification<TestEntity>
    {
        public TestEntityWithAddressSpecification()
        {
            AddInclude(x => x.Address);
        }
    }
}