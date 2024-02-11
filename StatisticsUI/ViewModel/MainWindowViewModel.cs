using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using DataContextLib.Models;
using Microsoft.Win32;
using RepairOperationService;
using StatisticsUI.Command;

namespace StatisticsUI.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IRepairOperationService repairOperationService;
    private ObservableCollection<RepairOperation> repairOperations = [];
    public ICommand ExportCommand { get; private set; }

    public ObservableCollection<RepairOperation> RepairOperations
    {
        get => repairOperations;
        set { repairOperations = value; OnPropertyChanged(); }
    }

    public MainWindowViewModel(IRepairOperationService service)
    {
        repairOperationService = service;
        RepairOperations = new ObservableCollection<RepairOperation>(service.GetAllOperations().GetAwaiter().GetResult());
        ExportCommand = new RelayCommandAsync(async () => await ExportData());
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public async Task FilterByEmployee(string? employeeName)
    {
        if (employeeName == null)
        {
            return;
        }
        RepairOperations = new ObservableCollection<RepairOperation>(await repairOperationService.GetOperationsByEmployee(employeeName));
    }

    public async Task FilterByDate(int year, int month) =>
        RepairOperations = new ObservableCollection<RepairOperation>(await repairOperationService.GetOperationsByDate(year, month));

    public async Task FilterByWorkType(string workType) =>
        RepairOperations = new ObservableCollection<RepairOperation>(await repairOperationService.GetOperationsByWorkType(workType));

    public async Task FilterByStatus(FaultReportStatus status) =>
       RepairOperations = new ObservableCollection<RepairOperation>(await repairOperationService.GetOperationsByStatus(status));

    public async Task ClearFilter() =>
       RepairOperations = new ObservableCollection<RepairOperation>(await repairOperationService.GetAllOperations());

    public async Task<IEnumerable<Employee>> GetAllEmployees() =>
        await repairOperationService.GetAllEmployees();

    public async Task<IEnumerable<RepairOperationType>> GetAllWorkTypes() =>
        await repairOperationService.GetAllRepairOperationTypes();

    private async Task ExportData()
    {
        ExportType[] exportTypes = 
        {
            new(".json", "JSON files(*.json)| *.json", ExportToJsonAsync),
            new(".xml", "XML files (*.xml)|*.xml", ExportToXmlAsync),
            new(".csv", "CSV files (*.csv)|*.csv", ExportToCsvAsync)
        };
        var filterString = string.Join("|", exportTypes.Select(et=>et.FilterText));
        SaveFileDialog saveFileDialog = new()
        {
            Filter = filterString,
            DefaultExt = Path.GetExtension(exportTypes.Select(et => et.Extension).First()),
            FileName = "ExportedData"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            string filePath = saveFileDialog.FileName;
            string selectedExtension = Path.GetExtension(filePath).ToLower();
            var type = exportTypes.Where(et => et.Extension == selectedExtension).FirstOrDefault();
            if (type is not null)
            {
                await type.ExportFunc(filePath);
            }
            else
            {
                MessageBox.Show("Unsupported file type.");
            }
        }
    }

    private async Task ExportToJsonAsync(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles

        };
        string json = JsonSerializer.Serialize(RepairOperations, options);
        try
        {
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving file: {ex.Message}");
        }
    }

    private async Task ExportToXmlAsync(string filePath)
    {
        var serializer = new XmlSerializer(typeof(ObservableCollection<RepairOperation>));

        await using (var memoryStream = new MemoryStream())
        {
            try
            {
                serializer.Serialize(memoryStream, RepairOperations);

                memoryStream.Seek(0, SeekOrigin.Begin);

                await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await memoryStream.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
        }
    }

    private async Task ExportToCsvAsync(string filePath)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Id;Employee;StartDate;EndDate;Status;WorkType;Description;Address");
        foreach (var operation in RepairOperations)
        {
            sb.AppendLine($"{operation.Id};{operation.Employee};{operation.StartDate};{operation.EndDate};{operation.FaultReport?.Status};{operation.OperationType};{operation.FaultReport?.Description};{operation.FaultReport?.Address}");
        }
        try
        {
            using var writer = new StreamWriter(filePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            await writer.WriteAsync(sb.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving file: {ex.Message}");
        }
    }

    private record ExportType(string Extension, string FilterText, Func<string, Task> ExportFunc);
}
