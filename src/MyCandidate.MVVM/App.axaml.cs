using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using MyCandidate.MVVM.ViewModels;
using MyCandidate.MVVM.Views;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using MyCandidate.Common;
using MyCandidate.DataAccess;
using MyCandidate.MVVM.Themes;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.ViewModels.Dictionary;
using log4net;
using System.Reflection;
using MyCandidate.MVVM.Services;

namespace MyCandidate.MVVM;

public partial class App : Application
{
    private const string DATA_DIRECTORY = "DATA_DIRECTORY";
    private IHost? _host;
    private CancellationTokenSource? _cancellationTokenSource;    
    public static IThemeManager? ThemeManager;
    public override void Initialize()
    {
        ThemeManager = new FluentThemeManager();
        ThemeManager.Initialize(this);
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.SetData("DataDirectory", AppSettings.AppDataPath);

            var dataDirectory = Environment.GetEnvironmentVariable(DATA_DIRECTORY);
            if (dataDirectory == null)
            {
                Environment.SetEnvironmentVariable(DATA_DIRECTORY, AppSettings.AppDataPath);
            }

            var builder = Host.CreateApplicationBuilder();

            builder.Configuration
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(AppSettings.JSON_FILE_NAME, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Logging
                .ClearProviders()
                .AddLog4Net();

            IServiceCollection services = builder.Services;

            services
                .Configure<AppSettings>(builder.Configuration)
                .AddSingleton<ILog>(LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType))
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<MainWindow>(service => new MainWindow
                {
                    DataContext = service.GetRequiredService<MainWindowViewModel>()
                })
                //data access 
                .AddTransient<IDatabaseCreator, DatabaseCreator>()           
                .AddTransient<IDataAccess<Country>, Countries>()
                .AddTransient<IDataAccess<City>, Cities>()
                .AddTransient<IDataAccess<Skill>, Skills>()
                .AddTransient<IDataAccess<SkillCategory>, SkillCategories>()
                .AddTransient<IDataAccess<Company>, Companies>()
                .AddTransient<IDataAccess<Office>, Officies>()
                .AddTransient<ICandidates, Candidates>()
                .AddTransient<IDictionariesDataAccess, Dictionaries>()
                .AddTransient<IVacancies, Vacancies>()
                .AddTransient<ICandidateSkills, CandidateSkills>()
                .AddTransient<IVacancySkills, VacancySkills>()
                .AddTransient<ICandidateOnVacancies, CandidateOnVacancies>()
                .AddTransient<IComments, Comments>()
                //services
                .AddTransient<IDictionaryService<Country>, CountryService>()
                .AddTransient<IDictionaryService<City>, CityService>()
                .AddTransient<IDictionaryService<Skill>, SkillService>()
                .AddTransient<IDictionaryService<SkillCategory>, SkillCategoryService>()
                .AddTransient<IDictionaryService<Company>, CompanyService>()
                .AddTransient<IDictionaryService<Office>, OfficeService>()
                .AddTransient<ICandidateService, CandidateService>()
                .AddTransient<IVacancyService, VacancyService>()
                .AddTransient<IAppServiceProvider, AppServiceProvider>()
                //ViewModels
                .AddTransient<DictionaryViewModel<Country>, CountriesViewModel>()
                .AddTransient<DictionaryViewModel<City>, CitiesViewModel>()
                .AddTransient<DictionaryViewModel<Skill>, SkillsViewModel>()
                .AddTransient<DictionaryViewModel<SkillCategory>, CategoriesViewModel>()
                .AddTransient<DictionaryViewModel<Company>, CompaniesViewModel>()
                .AddTransient<DictionaryViewModel<Office>, OfficiesViewModel>();

            _host = builder.Build();
            _cancellationTokenSource = new();

            try
            {
                GetRequiredService<IDatabaseCreator>().CreateDatabase();
                // set and show
                desktop.MainWindow = GetRequiredService<MainWindow>();
                desktop.ShutdownRequested += OnShutdownRequested;

                // startup background services
                _ = _host.StartAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // skip
            }
            catch (Exception ex)
            {
                Logger!.LogError(ex, ex.Message);
                ShowMessageBox("Unhandled Error", ex.Message);
                return;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    #region Logger
    private ILogger<App>? _logger;
    private ILogger<App>? Logger
    {
        get
        {
            if (_logger == null)
            {
                _logger = GetRequiredService<ILogger<App>>();
            }

            return _logger;
        }
    }
    #endregion

    public T GetRequiredService<T>() => _host!.Services.GetRequiredService<T>();

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        => _ = _host!.StopAsync(_cancellationTokenSource!.Token);

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = (Exception)e.ExceptionObject;
        Logger!.LogError(ex, ex.Message);
        ShowMessageBox("Unhandled Error", ex.Message);
    }

    private void ShowMessageBox(string title, string message)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(
            title, message, ButtonEnum.Ok, Icon.Stop);
        messageBoxStandardWindow.ShowAsync();
    }
}