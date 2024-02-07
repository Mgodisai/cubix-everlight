using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataContextLib;
using DataContextLib.Models;
using DataContextLib.UnitOfWorks;
using StatisticsUI.Command;

namespace StatisticsUI.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IUnitOfWork<DataDbContext> _unitOfWork;
    public ObservableCollection<RepairOperation> RepairOperations { get; set; }

    public MainViewModel()
    {

    }

    public MainViewModel(IUnitOfWork<DataDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
        RepairOperations = new ObservableCollection<RepairOperation>();
        LoadCommands();
    }

    public ICommand QueryCommand { get; private set; }
    public ICommand ExportCommand { get; private set; }

    private void LoadCommands()
    {
        QueryCommand = new RelayCommand(PerformQuery);
        ExportCommand = new RelayCommand(ExportData);
    }

    private void PerformQuery(object parameter)
    {
        // Implementáld a lekérdezés logikáját itt
        // Például: RepairOperations.Clear(); és adatok betöltése az ObservableCollection-be
    }

    private void ExportData(object parameter)
    {
        // Implementáld az adatok exportálásának logikáját itt
    }

    // Implementáld az INotifyPropertyChanged interfészt
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

