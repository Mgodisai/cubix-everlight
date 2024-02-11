using Common;
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
        var dbFilePath = Path.Combine(appDataDirectory, Strings.General_AppDataDirectory, Strings.General_DbName);
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

        var address1 = new Address { PostalCode = "7400", City = "Kaposvár", Street = "Virág u.", HouseNumber = "17", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var address2 = new Address { PostalCode = "7400", City = "Kaposvár", Street = "Hegyi u.", HouseNumber = "8/A", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var address3 = new Address { PostalCode = "1011", City = "Budapest", Street = "Fő utca", HouseNumber = "1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var address4 = new Address { PostalCode = "1014", City = "Budapest", Street = "Margit körút", HouseNumber = "5/B", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var address5 = new Address { PostalCode = "1117", City = "Budapest", Street = "Október huszonharmadika utca", HouseNumber = "8", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var address6 = new Address { PostalCode = "1117", City = "Budapest", Street = "Szent István körút", HouseNumber = "22", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var address7 = new Address { PostalCode = "1118", City = "Budapest", Street = "Kazinczy utca", HouseNumber = "52", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        modelBuilder.Entity<Address>().HasData(
           address1, address2, address3, address4, address5, address6, address7
        );

        var faultReport1 = new FaultReport { ReportedAt = new DateTime(2024, 1, 1), AddressId = address1.Id, Description = "Az utca végén lévő lámpaoszlop már több hete nem világít este, sötétben nagyon veszélyes az úton közlekedni.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var faultReport2 = new FaultReport { ReportedAt = new DateTime(2024, 1, 2), AddressId = address1.Id, Description = "A park melletti utcai lámpa folyamatosan villog, ami zavarja a környék lakóit és a gyalogosokat egyaránt.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        var faultReport3 = new FaultReport { ReportedAt = new DateTime(2024, 1, 15), AddressId = address2.Id, Description = "Az egyik utcai lámpa ernyője eltűnt, így a fény közvetlenül süt az ablakomba éjszaka, megzavarva az alvásomat.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var faultReport4 = new FaultReport { ReportedAt = new DateTime(2024, 1, 15), AddressId = address2.Id, Description = "A viharos szél ledöntött egy utcai lámpaoszlopot a járdán, ami most akadályozza a gyalogos forgalmat és balesetveszélyes.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        var faultReport5 = new FaultReport { ReportedAt = new DateTime(2024, 1, 31), AddressId = address3.Id, Description = "A gyalogos átjáró feletti közvilágítás nem működik, este nagyon nehéz észrevenni a gyalogosokat.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        var faultReport6 = new FaultReport { ReportedAt = new DateTime(2023, 11, 12), AddressId = address4.Id, Description = "Az utcai világítás túl korán kialszik este, még mielőtt az utca teljesen sötét lenne, ami biztonsági kockázatot jelent.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var faultReport7 = new FaultReport { ReportedAt = new DateTime(2023, 12, 1), AddressId = address4.Id, Description = "Több utcai lámpa is villog a főúton, ami zavaró lehet a közlekedők számára és potenciálisan veszélyes a vezetés során.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        var faultReport8 = new FaultReport { ReportedAt = new DateTime(2024, 2, 1), AddressId = address5.Id, Description = "A járda melletti utcai lámpák egy része nem kapcsol be esténként, ami balesetveszélyes sötétséget okoz a sétálók számára.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var faultReport9 = new FaultReport { ReportedAt = new DateTime(2024, 2, 2), AddressId = address5.Id, Description = "A közparkban lévő világítás egy része hibás, néhány lámpa egyáltalán nem világít.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        var faultReport10 = new FaultReport { ReportedAt = new DateTime(2024, 2, 3), AddressId = address6.Id, Description = "A közterületi világítás az egész utcában hirtelen kialszik éjszaka, ami teljes sötétséget eredményez és növeli a bűncselekmények kockázatát.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        var faultReport11 = new FaultReport { ReportedAt = new DateTime(2024, 2, 3), AddressId = address7.Id, Description = "Az újtelepi lakóparkban több utcai lámpa is meghibásodott, és már hetek óta nem javították meg, ami nehézséget okoz a késő este hazatérő lakóknak.", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        modelBuilder.Entity<FaultReport>().HasData(
            faultReport1, faultReport2, faultReport3, faultReport4, faultReport5, faultReport6, faultReport7, faultReport8, faultReport9, faultReport10, faultReport11
        );

        Guid guidForPos1 = Guid.NewGuid();
        var pos1 = new Position { Id = guidForPos1, Name = "Technician", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var pos2 = new Position { Name = "Engineer", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var pos3 = new Position { Name = "CEO", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        modelBuilder.Entity<Position>().HasData(
            pos1, pos2, pos3
        );

        modelBuilder.Entity<RepairOperationType>().HasData(
            new RepairOperationType { Name = "Undefined", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Username = "test123",
                PasswordHash = "$2a$11$nKDwqumqKe9QD0u94GdT3.eI.ab2lrr7Vn5ROIFQZ1pfEEi1jzGHG",
                DisplayName = "Test Employee00",
                Email = "test@test.hu",
                PositionId = guidForPos1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            }
        );
    }
}