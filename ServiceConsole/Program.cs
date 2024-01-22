using Data;
using Data.Models;
using Data.Repository;
using Microsoft.EntityFrameworkCore;
using ServiceConsole.Specifications;
using System.Text.RegularExpressions;
using static Common.ConsoleExtensions;

namespace ServiceConsole
{
    internal partial class Program
    {
        private const string regexPatternBudapestDistrict = @"^(1(0\d|1\d|2[0-3]))$";
        private const string regexPatternHungarianPostalCode = @"^[1-9]\d{3}$";
        private const string regexPatternForDays = @"^\d{1,2}$";
        static void Main(string[] args)
        {
            var dbContextOptions = ConfigureDbContextOptions();
            using var context = new DataDbContext(dbContextOptions);
            InitializeDatabase(context);
            RunFaultReportQueryLoop(context);
        }

        private static IEnumerable<FaultReport> ExecuteQuery(string input, QueryType queryType, IRepository<FaultReport> repository)
        {
            switch (queryType)
            {
                case QueryType.DistrictOfBudapest:
                    return repository.FindWithSpecification(new FaultReportByDistrictSpecification(input));
                case QueryType.HungarianPostalCode:
                    return repository.FindWithSpecification(new FaultReportByNormalPostalCodeSpecification(input));
                case QueryType.Days:
                    var result = int.TryParse(input, out int days);
                    if (!result) break;
                    return repository.FindWithSpecification(new FaultReportByDaysBeforeTodaySpecification(days));
                case QueryType.Invalid:
                default:
                    break;
            }
            return new List<FaultReport>();
        }

        private static void ProcessUserQuery(string input, IRepository<FaultReport> repository)
        {
            var queryType = QueryTypeSelector(input);
            if (queryType != QueryType.Invalid)
            {
                var result = ExecuteQuery(input, queryType, repository);
                if (result.Any())
                {
                    foreach (var fr in result)
                    {
                        WriteLineSuccess(fr.ToString());
                    }
                }
                else
                {
                    WriteLineError("Nincs találat");
                }
            }
            else
            {
                WriteLineError("Hibás lekérdezés");
            }
        }

        private static DbContextOptions<DataDbContext> ConfigureDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDirectory = Path.Combine(appDataDirectory, "EverlightApp");
            string dbFilePath = Path.Combine(appDirectory, "el.db");

            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            optionsBuilder.UseSqlite($"Data Source={dbFilePath}");
            return optionsBuilder.Options;
        }

        private static void InitializeDatabase(DataDbContext context)
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();
        }

        private static string GetUserInput()
        {
            Console.WriteLine("Hibabejelentések lekérdezése irányítószám (9999 formátum) vagy napok (<100) alapján (q: kilépés)");
            return Console.ReadLine() ?? string.Empty;
        }

        private static void RunFaultReportQueryLoop(DataDbContext context)
        {
            var faultReportRepository = new DataRepository<FaultReport>(context);

            while (true)
            {
                string input = GetUserInput();
                if (input.ToLower().Equals("q")) break;

                ProcessUserQuery(input, faultReportRepository);
            }
        }

        private static QueryType QueryTypeSelector(string? input)
        {
            if (BudapestDistrictRegex().IsMatch(input ?? string.Empty))
            {
                return QueryType.DistrictOfBudapest;
            }
            else if (HungarianPostalCodeRegex().IsMatch(input ?? string.Empty))
            {
                return QueryType.HungarianPostalCode;
            }
            else if (DaysPatternRegex().IsMatch(input ?? string.Empty))
            {
                return QueryType.Days;
            }
            else
            {
                return QueryType.Invalid;
            }
        }

        [GeneratedRegex(regexPatternBudapestDistrict)]
        private static partial Regex BudapestDistrictRegex();
        [GeneratedRegex(regexPatternHungarianPostalCode)]
        private static partial Regex HungarianPostalCodeRegex();
        [GeneratedRegex(regexPatternForDays)]
        private static partial Regex DaysPatternRegex();
    }

    public enum QueryType
    {
        HungarianPostalCode,
        DistrictOfBudapest,
        Days,
        Invalid = 99
    }
}
