//using Common;
//using DataContextLib.Models;
//using Microsoft.EntityFrameworkCore;
//using ServiceConsole.Service;
//using ServiceConsole.Specifications;
//using System.Text;
//using System.Text.RegularExpressions;
//using DataContextLib;
//using DataContextLib.Repository;
//using static Common.ConsoleExtensions;

//namespace ServiceConsole;

//internal partial class Program
//{
//    private const string regexPatternBudapestDistrict = @"^(1(0\d|1\d|2[0-3]))$";
//    private const string regexPatternHungarianPostalCode = @"^[1-9]\d{3}$";
//    private const string regexPatternForDays = @"^\d{1,2}$";
//    static int Main2(string[] args)
//    {
//        WriteLineSuccess(Strings.General_CompanyName + " - " + Strings.ServiceConsole_AppName);
//        using var dbContext = ConfigureDbContext();
//        var authService = new ConsoleAuthService(new DataRepository<Employee>(dbContext, null));

//        var authResult = PerformAuthentication(authService);

//        if (authResult.IsAuthenticated)
//        {
//            Console.Clear();
//            WriteLineSuccess(Strings.General_SuccessfulLogin);
//            WriteLineSuccess(String.Format(Strings.ServiceConsole_Greeting, authResult.DisplayName, authResult.Username));
//        }
//        else
//        {
//            WriteLineError(Strings.Error_BadCredentials);
//            return 1;
//        }

//        RunFaultReportQueryLoop(dbContext);
//        return 0;
//    }

//    private static AuthenticationResult PerformAuthentication(ConsoleAuthService authService)
//    {
//        WriteWarning(Strings.General_Username + ": ");
//        var username = Console.ReadLine() ?? string.Empty;

//        WriteWarning(Strings.General_Password + ": ");
//        var password = ReadPassword();

//        return authService.Authenticate(username, password);
//    }

//    private static string ReadPassword()
//    {
//        var password = new StringBuilder();
//        while (true)
//        {
//            var key = Console.ReadKey(intercept: true);
//            if (key.Key == ConsoleKey.Enter) break;
//            if (key.Key == ConsoleKey.Backspace && password.Length > 0) password.Remove(password.Length - 1, 1);
//            else if (key.Key != ConsoleKey.Backspace) password.Append(key.KeyChar);
//        }

//        Console.WriteLine();
//        return password.ToString();
//    }

//    private static async Task<IEnumerable<FaultReport>> ExecuteQueryAsync(string input, QueryType queryType, IRepository<FaultReport> repository)
//    {
//        switch (queryType)
//        {
//            case QueryType.DistrictOfBudapest:
//                return await repository.FindWithSpecificationAsync(new FaultReportByDistrictSpecification(input));
//            case QueryType.HungarianPostalCode:
//                return await repository.FindWithSpecificationAsync(new FaultReportByNormalPostalCodeSpecification(input));
//            case QueryType.Days:
//                var result = int.TryParse(input, out int days);
//                if (!result) break;
//                return await repository.FindWithSpecificationAsync(new FaultReportByDaysBeforeTodaySpecification(days));
//            case QueryType.Invalid:
//            default:
//                break;
//        }
//        return new List<FaultReport>();
//    }

//    private static void ProcessUserQuery(string input, IRepository<FaultReport> repository)
//    {
//        var queryType = QueryTypeSelector(input);
//        if (queryType != QueryType.Invalid)
//        {
//            var result = ExecuteQueryAsync(input, queryType, repository).ConfigureAwait(false).GetAwaiter().GetResult();
//            if (result.Any())
//            {
//                foreach (var fr in result)
//                {
//                    if (fr.Status == FaultReportStatus.New)
//                        WriteLineSuccess(fr.ToString());
//                    if (fr.Status == FaultReportStatus.InProgress)
//                        WriteLineWarning(fr.ToString());
//                }
//            }
//            else
//            {
//                WriteLineError(Strings.General_NoResult);
//            }
//        }
//        else
//        {
//            WriteLineError(Strings.General_BadQuery);
//        }
//    }

//    private static DataDbContext ConfigureDbContext()
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
//        string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
//        string appDirectory = Path.Combine(appDataDirectory, Strings.General_AppDataDirectory);
//        string dbFilePath = Path.Combine(appDirectory, Strings.General_DbName);

//        if (!Directory.Exists(appDirectory))
//        {
//            Directory.CreateDirectory(appDirectory);
//        }

//        optionsBuilder.UseSqlite($"Data Source={dbFilePath}");
//        var context = new DataDbContext(optionsBuilder.Options);
//        context.Database.EnsureCreated();
//        context.Database.Migrate();
//        return context;
//    }

//    private static string GetUserInput()
//    {
//        Console.WriteLine(string.Format(Strings.ServiceConsole_Info, Strings.ServiceConsole_Quit));
//        return Console.ReadLine() ?? string.Empty;
//    }

//    private static void RunFaultReportQueryLoop(DataDbContext context)
//    {
//        var faultReportRepository = new DataRepository<FaultReport>(context, null);

//        while (true)
//        {
//            string input = GetUserInput();
//            if (input.ToLower().Equals(Strings.ServiceConsole_Quit)) break;

//            ProcessUserQuery(input, faultReportRepository);
//        }
//    }

//    private static QueryType QueryTypeSelector(string? input)
//    {
//        if (BudapestDistrictRegex().IsMatch(input ?? string.Empty))
//        {
//            return QueryType.DistrictOfBudapest;
//        }
//        else if (HungarianPostalCodeRegex().IsMatch(input ?? string.Empty))
//        {
//            return QueryType.HungarianPostalCode;
//        }
//        else if (DaysPatternRegex().IsMatch(input ?? string.Empty))
//        {
//            return QueryType.Days;
//        }
//        else
//        {
//            return QueryType.Invalid;
//        }
//    }

//    [GeneratedRegex(regexPatternBudapestDistrict)]
//    private static partial Regex BudapestDistrictRegex();
//    [GeneratedRegex(regexPatternHungarianPostalCode)]
//    private static partial Regex HungarianPostalCodeRegex();
//    [GeneratedRegex(regexPatternForDays)]
//    private static partial Regex DaysPatternRegex();
//}

//public enum QueryType
//{
//    HungarianPostalCode,
//    DistrictOfBudapest,
//    Days,
//    Invalid = 99
//}