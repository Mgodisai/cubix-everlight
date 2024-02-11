using Common;
using static Common.ConsoleExtensions;
using System.Text.Json;
using ClientWebPortal.Models.Dtos;

namespace ServiceConsole;

internal partial class Program
{
    static async Task Main(string[] args)
    {
        WriteLineSuccess(Strings.General_CompanyName + " - " + Strings.ServiceConsole_AppName);
        await RunFaultReportQueryLoop();
    }

    private static JsonSerializerOptions GetOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private static async Task ProcessUserQuery(string input, JsonSerializerOptions options)
    {
        var baseUrl = "https://localhost:7199/api/FaultReport";
        using var client = new HttpClient();
        try
        {
            var uriBuilder = new UriBuilder(baseUrl)
            {
                Query = $"specialQuery={input}"
            };

            HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var faultReports = JsonSerializer.Deserialize<IEnumerable<FaultReportDto>>(content, options);

                if (faultReports is not null && faultReports.Any())
                {
                    foreach (var fr in faultReports)
                    {
                        var address = $"{fr.AddressDto?.PostalCode} {fr.AddressDto?.City}, {fr.AddressDto?.Street} {fr.AddressDto?.HouseNumber}";
                        var print = $"FaultReport ({fr.Status}): {address} - {fr.Description.TruncateString(20)}, reported: {fr.ReportedAt}, guid: {fr.Id}";

                            WriteLineSuccess(print);
                    }
                }
                else
                {
                    WriteLineError(Strings.General_NoResult);
                }
            }
            else
            {
                Console.WriteLine($"Failed to fetch data. Status code: {response.StatusCode}");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request exception: {e.Message}");
        }
    }

    private static string GetUserInput()
    {
        Console.WriteLine(string.Format(Strings.ServiceConsole_Info, Strings.ServiceConsole_Quit));
        return Console.ReadLine() ?? string.Empty;
    }

    private static async Task RunFaultReportQueryLoop()
    {
        while (true)
        {
            string input = GetUserInput();
            if (input.ToLower().Equals(Strings.ServiceConsole_Quit)) break;

            await ProcessUserQuery(input, GetOptions());
        }
    }
}
