using static Common.ConsoleExtensions;
using ConsoleAuthenticationService;
using DataContextLib;
using DataContextLib.UnitOfWorks;

namespace RepairOperationConsole.Services;

internal class UserActionService
{
    private readonly RepairOperationService.RepairOperationService service;
    private readonly AuthenticationResult userAuth;
    public UserActionService(DataDbContext dbContext, AuthenticationResult authResult)
    {
        IUnitOfWork<DataDbContext> unitOfWork = new UnitOfWork<DataDbContext>(dbContext);
        this.service = new RepairOperationService.RepairOperationService(unitOfWork);
        userAuth = authResult;
    }

    public UserAction GetUserAction()
    {
        Console.WriteLine("\nOpciók:");
        Console.WriteLine("1. Munkafelvétel");
        Console.WriteLine("2. Felvett munkák");
        Console.WriteLine("3. Munka lezárása");
        Console.WriteLine("4. Kilépés");
        Console.Write(": ");

        return ParseUserAction(Console.ReadLine() ?? string.Empty);
    }

    private UserAction ParseUserAction(string input)
    {
        switch (input)
        {
            case "1": return UserAction.TakeFaultReport;
            case "2": return UserAction.ListTakenReports;
            case "3": return UserAction.CompleteFaultReport;
            case "4": return UserAction.Logout;
            default:
                return UserAction.Invalid;
        }
    }

    public async Task ExecuteAction(UserAction action)
    {
        switch (action)
        {
            case UserAction.TakeFaultReport:
                Console.WriteLine("Munkafelvétel:");
                await TakeFaultReport();
                break;
            case UserAction.ListTakenReports:
                Console.WriteLine("Felvett munkák listázása");
                await ListTakenReports();
                break;
            case UserAction.CompleteFaultReport:
                Console.WriteLine("Munka lezárása");
                await CompleteFaultReport();
                break;
            case UserAction.Logout:
                Console.WriteLine("Kilépés");
                break;
            case UserAction.Invalid:
            default:
                Console.WriteLine("Hibás opció!");
                break;
        }
    }

    private async Task CompleteFaultReport()
    {
        WriteLineWarning("Adja meg a munka azonosítóját (guid): ");
        var repairOperationGuid = Console.ReadLine();
        WriteLineWarning("Adja meg az elvégzett javítás típusát: ");
        var operation = Console.ReadLine() ?? "Undefined";
        if (Guid.TryParse(repairOperationGuid, out var parsedGuid))
        {
            try
            {
                await service.CompletedRepairOperationAsync(parsedGuid, userAuth.Employee, operation);
                WriteLineSuccess($"Munka lezárva: {repairOperationGuid} ({operation}) - {userAuth?.Employee?.DisplayName}");

            }
            catch (InvalidOperationException ex)
            {
                WriteLineError(ex.Message);
            }
        }
        else
        {
            WriteLineError("Hibás Guid");
        }
    }

    private async Task ListTakenReports()
    {
        var listOfTakenReports = await service.ListTakenReportsByEmployee(userAuth.Employee);
        foreach (var report in listOfTakenReports)
        {
            WriteLineSuccess($"Guid: {report.Id}, leírás: {report.FaultReport?.Description}, státusz: {report.FaultReport?.Status}, op.type: {report.OperationType?.Name}");
        }
    }

    private async Task TakeFaultReport()
    {
        WriteLineWarning("Adja meg a hibajelentés azonosítóját (guid): ");
        var faultReportGuid = Console.ReadLine();
        if (Guid.TryParse(faultReportGuid, out var parsedGuid))
        {
            try
            {
                _ = await service.AssignFaultReportToEmployee(parsedGuid, userAuth.Employee);
                WriteLineSuccess($"Hiba felvéve: {faultReportGuid} - {userAuth?.Employee?.DisplayName}");
            }
            catch (InvalidOperationException ex)
            {
                WriteLineError(ex.Message);
            }
        }
        else
        {
            WriteLineError("Hibás Guid");
        }

    }
}