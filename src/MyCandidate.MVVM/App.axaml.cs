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
using System.IO;
using System.Diagnostics;

namespace MyCandidate.MVVM;

public partial class App : Application
{
    private const string DATA_DIRECTORY = "DATA_DIRECTORY";
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
            if(dataDirectory == null)
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
                .AddTransient<IDataAccess<Country>, Countries>()
                .AddTransient<IDataAccess<City>, Cities>()
                .AddTransient<DictionaryViewModel<Country>, CountriesViewModel>()
                .AddTransient<DictionaryViewModel<City>, CitiesViewModel>();

            _host = builder.Build();
            _cancellationTokenSource = new();

            try
            {
                (new DatabaseCreator(_host!.Services.GetRequiredService<ILogger<DatabaseCreator>>())).CreateDatabase();
                // set and show
                desktop.MainWindow = _host.Services.GetRequiredService<MainWindow>();
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

    #region Fields

    private IHost? _host;
    private CancellationTokenSource? _cancellationTokenSource;
    private ILogger<App>? _logger; 
    private DictionaryViewModel<Country>? _countriesViewModel;
    private DictionaryViewModel<City>? _citiesViewModel;   

    #endregion 

    public ILogger<App>? Logger
    {
        get
        {
            if(_logger == null)
            {
                _logger = _host!.Services.GetRequiredService<ILogger<App>>();
            }
            
            return _logger;
        }
    } 

    
    public DictionaryViewModel<Country>? CountriesViewModel
    {
        get
        {
            if(_countriesViewModel == null)
            {
                _countriesViewModel = _host!.Services.GetRequiredService<DictionaryViewModel<Country>>();
            }
            return _countriesViewModel;
        }
    }

    public DictionaryViewModel<City>? CitiesViewModel
    {
        get
        {
            if(_citiesViewModel == null)
            {
                _citiesViewModel = _host!.Services.GetRequiredService<DictionaryViewModel<City>>();
            }
            return _citiesViewModel;
        }
    }    

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