using System.Windows.Input;

namespace StatisticsUI.Command
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;

        public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public async void Execute(object parameter)
        {
            await execute();
        }
    }

}
