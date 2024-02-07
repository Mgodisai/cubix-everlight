using Common;
using DataContextLib;
using DataContextLib.Models;
using DataContextLib.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ConsoleAuthenticationService;
using RepairOperationConsole.Services;
using static Common.ConsoleExtensions;

namespace RepairOperationConsole;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        WriteLineSuccess(Strings.General_CompanyName + " - " + Strings.ServiceConsole_AppName);
        using var dbContext = ConfigureDbContext();
        var authService = new ConsoleAuthService(new DataRepository<Employee>(dbContext, null));

        var authResult = PerformAuthentication(authService);

        if (authResult.IsAuthenticated)
        {
            var userActionService = new UserActionService(dbContext, authResult);
            Console.Clear();
            WriteLineSuccess(Strings.General_SuccessfulLogin);
            WriteLineSuccess(String.Format(Strings.ServiceConsole_Greeting, authResult.DisplayName, authResult.Username));

            UserAction action;
            do
            {
                action = userActionService.GetUserAction();
                await userActionService.ExecuteAction(action);
            } while (action != UserAction.Logout);
        }
        else
        {
            WriteLineError($"{Strings.Error_BadCredentials}: {authResult.Message}");
            return 1;
        }
        return 0;
    }

    private static AuthenticationResult PerformAuthentication(ConsoleAuthService authService)
    {
        WriteWarning(Strings.General_Username + ": ");
        var username = Console.ReadLine() ?? string.Empty;

        WriteWarning(Strings.General_Password + ": ");
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


    private static DataDbContext ConfigureDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
        string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appDirectory = Path.Combine(appDataDirectory, Strings.General_AppDataDirectory);
        string dbFilePath = Path.Combine(appDirectory, Strings.General_DbName);

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
}