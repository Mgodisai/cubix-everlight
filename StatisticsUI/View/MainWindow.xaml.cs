using DataContextLib.Models;
using StatisticsUI.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace StatisticsUI.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            rbEmployee.Checked += async (s, e) => await SetupEmployeeFilter();
            rbDate.Checked += (s, e) => SetupDateFilter();
            rbWorkType.Checked += async (s, e) => await SetupWorkTypeFilter();
            rbClear.Checked += async (s, e) => await SetupNoFilter();
            rbWorkStatus.Checked += (s, e) => SetupStatusFilter();
        }

        private async Task SetupEmployeeFilter()
        {
            panelFilters.Children.Clear();
            var comboBox = new ComboBox { Margin = new Thickness(0, 10, 0, 0) };

            comboBox.ItemsSource = ((MainWindowViewModel)this.DataContext).GetAllEmployees().Result.Select(e=>e.DisplayName);
            comboBox.SelectionChanged += async (s, e) =>
            {
                if (comboBox.SelectedItem != null)
                {
                    await ((MainWindowViewModel)this.DataContext).FilterByEmployee(comboBox.SelectedItem.ToString());
                }
            };
            panelFilters.Children.Add(comboBox);
        }

        private void SetupDateFilter()
        {
            panelFilters.Children.Clear();

            var datePicker = new DatePicker();
            datePicker.SelectedDateChanged += (s, e) =>
            {
                if (datePicker.SelectedDate.HasValue)
                {
                    ((MainWindowViewModel)this.DataContext).FilterByDate(datePicker.SelectedDate.Value);
                }
            };
            panelFilters.Children.Add(datePicker);
        }

        private async Task SetupWorkTypeFilter()
        {
            panelFilters.Children.Clear();
            var comboBox = new ComboBox { Margin = new Thickness(0, 10, 0, 0) };

            comboBox.ItemsSource = await ((MainWindowViewModel)this.DataContext).GetAllWorkTypes();
            comboBox.SelectionChanged += async (s, e) =>
            {
                if (comboBox.SelectedItem != null)
                {
                    await ((MainWindowViewModel)this.DataContext).FilterByWorkType(comboBox.SelectedItem.ToString());
                }
            };
            panelFilters.Children.Add(comboBox);
        }

        private void SetupStatusFilter()
        {
            panelFilters.Children.Clear();
            var comboBox = new ComboBox { Margin = new Thickness(0, 10, 0, 0) };

            comboBox.ItemsSource = Enum.GetNames(typeof(FaultReportStatus));
            comboBox.SelectionChanged += async (s, e) =>
            {
                if (comboBox.SelectedItem != null)
                {
                    var status = (FaultReportStatus)Enum.Parse(typeof(FaultReportStatus), comboBox.SelectedItem.ToString());
                    await ((MainWindowViewModel)this.DataContext).FilterByStatus(status);
                }
            };
            panelFilters.Children.Add(comboBox);
        }

        private async Task SetupNoFilter()
        {
            panelFilters.Children.Clear();

            await ((MainWindowViewModel)this.DataContext).ClearFilter();

        }
    }
}