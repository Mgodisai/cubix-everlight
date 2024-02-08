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
    private ObservableCollection<RepairOperation> repairOperations;
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
        ExportCommand = new RelayCommandAsync(async () => await ExportData("JSON"));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public async Task FilterByEmployee(string employeeName) =>
        RepairOperations = new ObservableCollection<RepairOperation>(await repairOperationService.GetOperationsByEmployee(employeeName));

    public async Task FilterByDate(DateTime date) =>
        RepairOperations = new ObservableCollection<RepairOperation>(await repairOperationService.GetOperationsByDate(date));

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

    private async void ExportData(object parameter)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml|CSV files (*.csv)|*.csv",
            DefaultExt = "json",
            FileName = "ExportedData"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            string exportType = parameter as string;
            switch (exportType)
            {
                case "JSON":
                    await ExportToJson(saveFileDialog.FileName);
                    break;
                case "XML":
                    await ExportToXml(saveFileDialog.FileName);
                    break;
                case "CSV":
                    await ExportToCsv(saveFileDialog.FileName);
                    break;
            }
        }
    }

    private void ExportToJson(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve
        };
        string json = JsonSerializer.Serialize(RepairOperations, options);
        try
        {
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving file: {ex.Message}");
        }
    }

    private void ExportToXml(string filePath)
    {
        var serializer = new XmlSerializer(typeof(ObservableCollection<RepairOperation>));
        using (var writer = new StreamWriter(filePath))
        {
            try
            {
                serializer.Serialize(writer, RepairOperations);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
        }
    }

    private void ExportToCsv(string filePath)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Id,EmployeeName,Date,WorkType");
        foreach (var operation in RepairOperations)
        {
            sb.AppendLine($"{operation.Id},{operation.Employee},{operation.StartDate},{operation.EndDate}");
        }
        try
        {
            File.WriteAllText(filePath, sb.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving file: {ex.Message}");
        }
        
    }
}
