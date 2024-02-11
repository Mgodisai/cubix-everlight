using DataContextLib.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static DataContextLib.Tests.DataRepositoryTests;

namespace DataContextLib.Tests
{
    public class TestDbContext : IdentityDbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
            Database.OpenConnection();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<TestEntity>()
                .HasOne(f => f.Address)
                .WithMany()
                .HasForeignKey(f => f.AddressId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
