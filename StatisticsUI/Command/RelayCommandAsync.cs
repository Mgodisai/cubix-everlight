using System.Windows.Input;

namespace StatisticsUI.Command;

public class RelayCommandAsync(Func<Task> execute, Func<bool>? canExecute = null) : ICommand
{
    private readonly Func<Task> execute = execute;
    private readonly Func<bool> canExecute = canExecute ?? (() => true);

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return canExecute == null || canExecute();
    }

    public async void Execute(object? parameter)
    {
        await execute();
    }
}