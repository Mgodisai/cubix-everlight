using Data.Models;
using DataContextLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{

    public class DataDbContext(DbContextOptions<DataDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<FaultReport> FaultReports { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Position> Positions { get; set; }
        public DbSet<RepairOperationType> RepairTypes { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<RepairOperation> RepairOperations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string dbFilePath = Path.Combine(appDataDirectory, "EverlightApp", "el.db");
                optionsBuilder.UseSqlite($"Data Source={dbFilePath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FaultReport>()
                .HasOne(f => f.Address)
                .WithMany()
                .HasForeignKey(f => f.AddressId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.PositionId);

            modelBuilder.Entity<RepairOperation>()
               .HasOne(ro => ro.Employee)
               .WithMany()
               .HasForeignKey(ro => ro.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepairOperation>()
                .HasOne(ro => ro.FaultReport)
                .WithMany()
                .HasForeignKey(ro => ro.FaultReportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepairOperation>()
                .HasOne(ro => ro.OperationType)
                .WithMany(ot => ot.RepairOperations)
                .HasForeignKey(ro => ro.OperationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            var address1 = new Address { PostalCode = "7400", City = "Kaposvár", Street = "Virág u.", HouseNumber = "17" };
            var address2 = new Address { PostalCode = "7400", City = "Kaposvár", Street = "Hegyi u.", HouseNumber = "8/A" };
            modelBuilder.Entity<Address>().HasData(
               address1, address2
            );

            modelBuilder.Entity<FaultReport>().HasData(
                new FaultReport { AddressId = address1.Id, Description = "Description1", ReportedAt = DateTime.Now.AddDays(-3)},
                new FaultReport { AddressId = address2.Id, Description = "Description2", ReportedAt = DateTime.Now.AddDays(-5) },
                new FaultReport { AddressId = address2.Id, Description = "Description3", ReportedAt = DateTime.Now.AddDays(-10), Status = FaultReportStatus.Completed }
            );

            var testPosGuid = Guid.NewGuid();
            modelBuilder.Entity<Position>().HasData(
                new Position { Name="CEO" },
                new Position { Name = "Technician", Id = testPosGuid },
                new Position { Name = "Engineer" }
            );

            modelBuilder.Entity<RepairOperationType>().HasData(
                new RepairOperationType { Name = "Light Bulb replacement" },
                new RepairOperationType { Name = "Lamp Shade replacement" },
                new RepairOperationType { Name = "Wire repair" },
                new RepairOperationType { Name = "Lamp support post replacement" }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee 
                { 
                    Username = "test123", 
                    PasswordHash = @"$2a$11$NjdCdbcEMyAVhb40DYQt/OBO..72ZxnWH8.biNBWkQJue4fA3bs.W", 
                    DisplayName="Test Emp", 
                    Email="test@test.hu", 
                    PositionId= testPosGuid
                }
            );
        }
    }
}
