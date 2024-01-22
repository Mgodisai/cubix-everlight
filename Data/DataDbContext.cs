using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{

    public class DataDbContext(DbContextOptions<DataDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<FaultReport> FaultReports { get; set; }
        public DbSet<Address> Addresses { get; set; }

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

            var address1 = new Address { PostalCode = "7400", City = "Kaposvár", Street = "Virág u.", HouseNumber = "17" };
            var address2 = new Address { PostalCode = "7400", City = "Kaposvár", Street = "Hegyi u.", HouseNumber = "8/A" };
            modelBuilder.Entity<Address>().HasData(
               address1, address2
            );

            modelBuilder.Entity<FaultReport>().HasData(
                new FaultReport { AddressId = address1.Id, Description = "Description1" },
                new FaultReport { AddressId = address2.Id, Description = "Description2" },
                new FaultReport { AddressId = address2.Id, Description = "Description3", Status = FaultReportStatus.Completed }
            );
        }
    }
}
