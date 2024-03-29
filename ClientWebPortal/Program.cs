using ClientWebPortal.Service;
using Microsoft.EntityFrameworkCore;
using ClientWebPortal.Mapping;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using ClientWebPortal.Resources;
using DataContextLib;
using DataContextLib.UnitOfWorks;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ClientWebPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                //builder.WebHost.UseUrls("http://*:80");

                logger.Debug("configure logging");
                ConfigureLogging(builder);

                logger.Debug("configure services");
                ConfigureServices(builder);

                logger.Debug("build app");
                var app = builder.Build();

                logger.Debug("configure middleware");
                ConfigureMiddleware(app);

                logger.Debug("app run");
                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in ASP.NET config!");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static void ConfigureLogging(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            builder.Host.UseNLog();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            ConfigureDatabase(builder);

            ConfigureIdentity(builder);
            builder.Services.AddLogging();
            ConfigureRepositories(builder);

            builder.Services.AddAutoMapper(typeof(FaultReportMappingProfile));
            builder.Services.AddAutoMapper(typeof(EmployeeMappingProfile));
            builder.Services.AddControllersWithViews()
                .AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(ValidationMessages));
                });

            ConfigureEmailService(builder);
            ConfigureLocalization(builder);
        }

        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DataDbContext>(options =>
            {
                var appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appDirectory = Path.Combine(appDataDirectory, "EverlightApp");
                var dbFilePath = Path.Combine(appDirectory, "el.db");
                if (!Directory.Exists(appDirectory))
                {
                    Directory.CreateDirectory(appDirectory);
                }
                options.UseSqlite($"Data Source={dbFilePath}");
            });
        }

        private static void ConfigureRepositories(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUnitOfWork<DataDbContext>>(
                   provider =>
                   {
                       var dbContext = provider.GetRequiredService<DataDbContext>();
                       return new UnitOfWork<DataDbContext>(dbContext);
                   });
            builder.Services.AddScoped<IFaultReportService, FaultReportService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
        }

        private static void ConfigureIdentity(WebApplicationBuilder builder)
        {
            builder.Services.AddDefaultIdentity<IdentityUser>(
                options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<DataDbContext>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;

                options.Events = new CookieAuthenticationEvents
                {
                    OnSigningIn = context =>
                    {
                        if (context.Properties.Items.TryGetValue("IsPersistent", out string? value))
                        {
                            var isPersistent = bool.Parse(value ?? "false");
                            context.Properties.IsPersistent = isPersistent;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        private static void ConfigureEmailService(WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            builder.Services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            builder.Services.AddSingleton<IEmailService, EmailService>();
        }

        private static void ConfigureLocalization(WebApplicationBuilder builder)
        {
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new ("hu-HU"),
                            new ("en-US")
                        };

                    options.DefaultRequestCulture = new RequestCulture("hu-HU");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();
                dbContext.Database.EnsureCreated();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
        }
    }
}
