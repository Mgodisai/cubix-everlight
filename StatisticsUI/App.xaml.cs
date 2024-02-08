using System.IO;
using Microsoft.Extensions.DependencyInjection;
using RepairOperationService;
using StatisticsUI.ViewModel;
using System.Windows;
using DataContextLib;
using DataContextLib.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using StatisticsUI.View;

namespace StatisticsUI;

public partial class App
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        ServiceCollection services = new();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IRepairOperationService, RepairOperationService.RepairOperationService>();
        services.AddScoped<IUnitOfWork<DataDbContext>, UnitOfWork<DataDbContext>>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dbFilePath = Path.Combine(appDataDirectory, "EverlightApp", "el.db");
        services.AddDbContext<DataContextLib.DataDbContext>(options =>
            options.UseSqlite($"Data Source={dbFilePath}"));
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var mainWindow = _serviceProvider.GetService<MainWindow>();
        mainWindow.Show();
    }
}