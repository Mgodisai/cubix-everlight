using Data;
using Data.Models;
using Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace ServiceConsole
{
    internal class Program
   {

      static void Main(string[] args)
      {
         string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
         string appDirectory = Path.Combine(appDataDirectory, "EverlightApp");
         string dbFilePath = Path.Combine(appDirectory, "el.db");
         if (!Directory.Exists(appDirectory))
         {
            Directory.CreateDirectory(appDirectory);
         }

         var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
         optionsBuilder.UseSqlite($"Data Source={dbFilePath}");

         using (var context = new DataDbContext(optionsBuilder.Options))
         {
            context.Database.EnsureCreated();
                context.Database.Migrate();

            var faultReportRepository = new DataRepository<FaultReport>(context);

            var allFaultReports = faultReportRepository.GetAll().Where(f => f.Status == FaultReportStatus.New);

            foreach (var report in allFaultReports)
            {
               context.Entry(report).Reference(r => r.Address).Load();
               Console.WriteLine($"Report Description: {report.Description}, Address: {report.Address}, Created: {report.ReportedAt}, Status: {report.Status}");
            }
         }
         Console.ReadKey();
      }
   }
}
