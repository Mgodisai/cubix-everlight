using DataContextLib.Models;
using MaterialDesignThemes.Wpf;
using StatisticsUI.ViewModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace StatisticsUI.View;

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
        rbEmployee.Checked += (s, e) => SetupEmployeeFilter();
        rbDate.Checked += (s, e) => SetupDateFilter();
        rbWorkType.Checked += async (s, e) => await SetupWorkTypeFilter();
        rbClear.Checked += async (s, e) => await SetupNoFilter();
        rbWorkStatus.Checked += (s, e) => SetupStatusFilter();
    }

    private void SetupEmployeeFilter()
    {
        panelFilters.Children.Clear();
        var comboBox = new ComboBox
        {
            Margin = new Thickness(0, 10, 0, 0),
            ItemsSource = ((MainWindowViewModel)this.DataContext).GetAllEmployees().Result.Select(e => e.DisplayName)
        };
        comboBox.SelectionChanged += async (s, e) =>
        {
            var selectedItem = comboBox.SelectedItem.ToString();
            if (selectedItem != null)
            {
                await ((MainWindowViewModel)this.DataContext).FilterByEmployee(selectedItem);
            }
        };
        comboBox.SelectedIndex = 0;
        panelFilters.Children.Add(comboBox);
    }

    private void SetupDateFilter()
    {
        panelFilters.Children.Clear();

        var stack = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        var cbYear = new ComboBox()
        {
            Margin = new Thickness(0, 10, 10, 0)
        };
        var cbMonth = new ComboBox()
        {
            Margin = new Thickness(0, 10, 10, 0),
            
        };
        Button filterButton = new()
        {
            Content = "Szűrés",
            Margin = new Thickness(5),
            Style = (Style)Application.Current.FindResource("MaterialDesignRaisedAccentButton"),
            Effect = new DropShadowEffect()
        };
        stack.Children.Add(cbYear);
        stack.Children.Add(cbMonth);
        stack.Children.Add(filterButton);
        LoadMonths(cbMonth);
        LoadYears(cbYear);

        filterButton.Click += async (s, e) =>
        {
            int selectedYear = (int)cbYear.SelectedValue;
            int selectedMonth = cbMonth.SelectedIndex + 1;
            await ApplyFilter(selectedYear, selectedMonth);
        };
        panelFilters.Children.Add(stack);
    }

    private async Task ApplyFilter(int year, int month)
    {
        await((MainWindowViewModel)this.DataContext).FilterByDate(year, month);
    }

    private async Task SetupWorkTypeFilter()
    {
        panelFilters.Children.Clear();
        var comboBox = new ComboBox
        {
            Margin = new Thickness(0, 10, 0, 0),
            ItemsSource = await ((MainWindowViewModel)this.DataContext).GetAllWorkTypes()
        };
        comboBox.SelectionChanged += async (s, e) =>
        {
            var selectedItem = comboBox.SelectedItem.ToString();
            if (selectedItem != null)
            {
                await ((MainWindowViewModel)this.DataContext).FilterByWorkType(selectedItem);
            }
        };
        comboBox.SelectedIndex = 0;
        panelFilters.Children.Add(comboBox);
    }

    private void SetupStatusFilter()
    {
        panelFilters.Children.Clear();
        var comboBox = new ComboBox
        {
            Margin = new Thickness(0, 10, 0, 0),
            ItemsSource = Enum.GetNames(typeof(FaultReportStatus))
        };
        comboBox.SelectionChanged += async (s, e) =>
        {
            var selectedItem = comboBox.SelectedItem.ToString();
            if (selectedItem != null)
            {
                var status = (FaultReportStatus)Enum.Parse(typeof(FaultReportStatus), selectedItem);
                await ((MainWindowViewModel)this.DataContext).FilterByStatus(status);
            }
        };
        comboBox.SelectedIndex = 0;
        panelFilters.Children.Add(comboBox);
    }

    private async Task SetupNoFilter()
    {
        panelFilters.Children.Clear();

        await ((MainWindowViewModel)this.DataContext).ClearFilter();

    }

    private static void LoadMonths(ComboBox c)
    {
        for (int month = 1; month <= 12; month++)
        {
            c.Items.Add(new DateTime(1, month, 1).ToString("MMMM"));
            if (DateTime.Now.Month == month)
            {
                c.SelectedIndex = (month-1);
            }
        }
    }

    private static void LoadYears(ComboBox c)
    {
        int currentYear = DateTime.Now.Year;
        for (int year = currentYear - 10; year <= currentYear; year++)
        {
            c.Items.Add(year);
            if (year == currentYear)
            {
                c.SelectedValue = year;
            }
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}