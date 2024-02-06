using DataContextLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataContextLib;

public class DataDbContext(DbContextOptions<DataDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<FaultReport>? FaultReports { get; set; }
    public DbSet<Address>? Addresses { get; set; }

    public DbSet<Position>? Positions { get; set; }
    public DbSet<RepairOperationType>? RepairTypes { get; set; }

    public DbSet<Employee>? Employees { get; set; }
    public DbSet<RepairOperation>? RepairOperations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dbFilePath = Path.Combine(appDataDirectory, "EverlightApp", "el.db");
        optionsBuilder.UseSqlite($"Data Source={dbFilePath}");
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
           .HasOne(ro => ro.Technician)
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

        var address1 = new Address { PostalCode = "7400", City = "Kaposvár", Street = "Virág u.", HouseNumber = "17", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow};
        var address2 = new Address { PostalCode = "7400", City = "Kaposvár", Street = "Hegyi u.", HouseNumber = "8/A", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        modelBuilder.Entity<Address>().HasData(
           address1, address2
        );

        modelBuilder.Entity<FaultReport>().HasData(
            new FaultReport { AddressId = address1.Id, Description = "Description1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new FaultReport { AddressId = address2.Id, Description = "Description2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new FaultReport { AddressId = address2.Id, Description = "Description3", Status = FaultReportStatus.Completed, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Position>().HasData(
            new Position { Name = "CEO", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Position { Name = "Technician", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Position { Name = "Engineer", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<RepairOperationType>().HasData(
            new RepairOperationType { Name = "Light Bulb replacement", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new RepairOperationType { Name = "Lamp Shade replacement", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new RepairOperationType { Name = "Wire repair", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new RepairOperationType { Name = "Lamp support post replacement", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        //modelBuilder.Entity<Employee>().HasData(
        //    new Employee 
        //    { 
        //        Username = "test123", 
        //        PasswordHash = "$11$XCSVi7LPkrp8aSrmvh3twOBQ/ZPI1Unzkj/jYk.vHbagPxC3AMiUe", 
        //        DisplayName="Test Emp", 
        //        Email="test@test.hu", 
        //        PositionId= new Guid("44A47808-8EEC-4436-948E-A21A891C28EF")
        //    }
        //);
    }
}
