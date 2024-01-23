using Data;
using Data.Models;
using Data.Repository;
using DataContextLib.Models;
using Microsoft.EntityFrameworkCore;
using ServiceConsole.Service;
using ServiceConsole.Specifications;
using System.Text;
using System.Text.RegularExpressions;
using static Common.ConsoleExtensions;

namespace ServiceConsole
{
    internal partial class Program
    {
        private const string regexPatternBudapestDistrict = @"^(1(0\d|1\d|2[0-3]))$";
        private const string regexPatternHungarianPostalCode = @"^[1-9]\d{3}$";
        private const string regexPatternForDays = @"^\d{1,2}$";
        static int Main(string[] args)
        {
            using var dbContext = ConfigureDbContext();
            var authService = new ConsoleAuthService(new DataRepository<Employee>(dbContext));

            var authResult = PerformAuthentication(authService);

            if (authResult.IsAuthenticated)
            {
                Console.Clear();
                WriteLineSuccess($"Sikeres bejelentkezés! Welcome, {authResult.DisplayName}");
            }
            else
            {
                WriteLineError("Hibás bejelentkezési adatok!");
                return 1;
            }

            RunFaultReportQueryLoop(dbContext);
            return 0;
        }

        private static AuthenticationResult PerformAuthentication(ConsoleAuthService authService)
        {
            WriteWarning("Username: ");
            var username = Console.ReadLine() ?? string.Empty;

            WriteWarning("Password: ");
            var password = ReadPassword();

            return authService.Authenticate(username, password);
        }

        private static string ReadPassword()
        {
            var password = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && password.Length > 0) password.Remove(password.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) password.Append(key.KeyChar);
            }

            Console.WriteLine();
            return password.ToString();
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
                        if (fr.Status==FaultReportStatus.New)
                            WriteLineSuccess(fr.ToString());
                        if (fr.Status == FaultReportStatus.InProgress)
                            WriteLineWarning(fr.ToString());
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

        private static DataDbContext ConfigureDbContext()
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
            var context = new DataDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
            context.Database.Migrate();
            return context;
        }

        private static string GetUserInput()
        {
            Console.WriteLine("Hibabejelentések lekérdezése (irányítószámok: 9999 | kerületek: 1xx | napok <100) - q: kilépés");
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
