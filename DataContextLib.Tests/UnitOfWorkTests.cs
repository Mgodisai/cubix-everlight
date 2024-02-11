using DataContextLib.Models;
using DataContextLib.Repository;
using DataContextLib.UnitOfWorks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using static DataContextLib.Tests.DataRepositoryTests;

namespace DataContextLib.Tests
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        private TestDbContext _context;
        private UnitOfWork<TestDbContext> _unitOfWork;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite("Filename=:memory:;Mode=Memory;Cache=Shared")
                .Options;
            _context = new TestDbContext(options);
            _unitOfWork = new UnitOfWork<TestDbContext>(_context);
        }

        [Test]
        public async Task TransactionManagement_ShouldCommitTransactionSuccessfully()
        {
            // Arrange
            var testEntity = new TestEntity { Name = "Test Entity", Address = new Address { City = "Test City" } };

            // Act
            await _unitOfWork.CreateTransactionAsync();
            await _unitOfWork.GetRepository<TestEntity>().InsertAsync(testEntity);
            await _unitOfWork.CommitAsync();

            // Assert
            var savedEntity = await _unitOfWork.GetRepository<TestEntity>().GetByIdAsync(testEntity.Id);
            savedEntity.Should().NotBeNull();
            savedEntity!.Name.Should().Be("Test Entity");
            savedEntity!.Address.Should().NotBeNull();
            savedEntity!.Address!.City.Should().Be("Test City");
        }

        [Test]
        public async Task SaveEntity_ShouldSaveAndRetrieveEntitySuccessfully()
        {
            // Arrange
            var address = new Address { City = "Test City" };
            var testEntity = new TestEntity { Name = "Test Name", AddressId = address.Id };

            // Act
            await _unitOfWork.GetRepository<Address>().InsertAsync(address);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.GetRepository<TestEntity>().InsertAsync(testEntity);
            await _unitOfWork.SaveAsync();

            // Assert
            var retrievedEntity = await _unitOfWork.GetRepository<TestEntity>().FindWithSpecificationAsync(new TestEntityWithAddressSpecification());
            retrievedEntity.Should().NotBeNull();
            retrievedEntity.Should().ContainSingle();
            retrievedEntity.First().Name.Should().Be("Test Name");
            retrievedEntity.First().Address.Should().NotBeNull();
            retrievedEntity.First().Address!.City.Should().Be("Test City");
        }

        [Test]
        public async Task UpdateEntity_ShouldUpdateEntitySuccessfully()
        {
            // Arrange
            var testEntity = new TestEntity { Name = "Original Name" };
            await _unitOfWork.GetRepository<TestEntity>().InsertAsync(testEntity);
            await _unitOfWork.SaveAsync();

            // Act
            testEntity.Name = "Updated Name";
            await _unitOfWork.GetRepository<TestEntity>().UpdateAsync(testEntity);
            await _unitOfWork.SaveAsync();

            // Assert
            var updatedEntity = await _unitOfWork.GetRepository<TestEntity>().GetByIdAsync(testEntity.Id);
            updatedEntity.Should().NotBeNull();
            updatedEntity!.Name.Should().Be("Updated Name");
        }

        [Test]
        public async Task DeleteEntity_ShouldRemoveEntitySuccessfully()
        {
            // Arrange
            var testEntity = new TestEntity { Name = "Test Entity" };
            await _unitOfWork.GetRepository<TestEntity>().InsertAsync(testEntity);
            await _unitOfWork.SaveAsync();

            // Act
            await _unitOfWork.GetRepository<TestEntity>().DeleteAsync(testEntity.Id);
            await _unitOfWork.SaveAsync();

            // Assert
            var deletedEntity = await _unitOfWork.GetRepository<TestEntity>().GetByIdAsync(testEntity.Id);
            deletedEntity.Should().BeNull();
        }

        [Test]
        public void GetRepository_ReturnsRepositoryForEntityType()
        {
            // Act
            var repository = _unitOfWork.GetRepository<TestEntity>();

            // Assert
            repository.Should().NotBeNull();
            repository.Should().BeAssignableTo<IRepository<TestEntity>>();
        }

        [Test]
        public async Task RollbackAsync_DiscardsChanges()
        {
            // Arrange
            var id = Guid.NewGuid();
            var testEntity = new TestEntity { Id=id, Name = "Initial Name" };
            _context.TestEntities.Add(testEntity);
            await _context.SaveChangesAsync();

            // Act
            await _unitOfWork.CreateTransactionAsync();
            testEntity.Name = "Updated Name";
            await _context.SaveChangesAsync();
            await _unitOfWork.RollbackAsync();
            var reloadedEntity = await _context.TestEntities.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

            // Assert
            reloadedEntity.Should().NotBeNull();
            reloadedEntity!.Name.Should().Be("Initial Name");
        }

        [Test]
        public async Task CommitAsync_WithoutBeginningTransaction_ThrowsInvalidOperationException()
        {
            // Act & Assert
            await FluentActions.Invoking(async () => await _unitOfWork.CommitAsync())
                               .Should().ThrowAsync<InvalidOperationException>()
                               .WithMessage("No active transaction");
        }

        [Test]
        public async Task RollbackAsync_WithoutBeginningTransaction_ThrowsInvalidOperationException()
        {
            // Act & Assert
            await FluentActions.Invoking(async () => await _unitOfWork.RollbackAsync())
                               .Should().ThrowAsync<InvalidOperationException>()
                               .WithMessage("No active transaction");
        }


        [TearDown]
        public void TearDown()
        {
            _unitOfWork?.Dispose();
        }
    }
}
