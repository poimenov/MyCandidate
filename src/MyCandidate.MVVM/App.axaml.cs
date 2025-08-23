using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia.Enums;
using MyCandidate.MVVM.Views;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using MyCandidate.Common;
using MyCandidate.MVVM.Themes;
using MyCandidate.Common.Interfaces;
using log4net;
using System.Reflection;
using MyCandidate.MVVM.Extensions;

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

            var configurationManager = new ConfigurationManager();
            configurationManager
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(AppSettings.JSON_FILE_NAME, optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            var settings = new HostApplicationBuilderSettings
            {
                ApplicationName = AppSettings.APPLICATION_NAME,
                Configuration = configurationManager
            };

            var builder = Host.CreateApplicationBuilder(settings);

            builder.Logging
                .ClearProviders()
                .AddLog4Net();

            IServiceCollection services = builder.Services;

            services
                .Configure<AppSettings>(builder.Configuration)
                .AddSingleton<ILog>(LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType))
                .AddApplicationServices(builder.Configuration);

            _host = builder.Build();
            _cancellationTokenSource = new();

            try
            {
                GetRequiredService<IDatabaseMigrator>().MigrateDatabase();
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

    public T GetRequiredService<T>() where T : notnull => _host!.Services.GetRequiredService<T>();
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
        var messageBoxStandardWindow = MessageBoxExtension.GetMessageBox(
                                            title, message, ButtonEnum.Ok, Icon.Stop);
        messageBoxStandardWindow.ShowAsync();
    }
}