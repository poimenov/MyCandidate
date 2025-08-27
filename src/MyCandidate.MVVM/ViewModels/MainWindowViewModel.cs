using System;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.PropertyGrid.Services;
using Dock.Model.Controls;
using Microsoft.Extensions.Options;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MyCandidate.Common;
using MyCandidate.MVVM.Extensions;
using MyCandidate.MVVM.Localizations;
using MyCandidate.MVVM.Services;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAppServiceProvider _provider;
    private readonly IOptions<AppSettings> _options;

    public MainWindowViewModel(IAppServiceProvider appServiceProvider, IOptions<AppSettings> options)
    {
        _provider = appServiceProvider;
        _options = options;

        LocalizationService.Default.AddExtraService(new AppLocalizationService());
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        Title = LocalizationService.Default["progName"];
        MenuThemeViewModel = new MenuThemeViewModel(_options.Value).DisposeWith(Disposables);
        MenuLanguageViewModel = new MenuLanguageViewModel(_options.Value).DisposeWith(Disposables);
        RecentCandidatesViewModel = new MenuRecentViewModel(_provider, Models.TargetModelType.Candidate, 10).DisposeWith(Disposables);
        RecentVacanciesViewModel = new MenuRecentViewModel(_provider, Models.TargetModelType.Vacancy, 5).DisposeWith(Disposables);

        var factory = appServiceProvider.Factory;
        Layout = factory?.CreateLayout();
        if (Layout is { })
        {
            factory?.InitLayout(Layout);
        }

        FileExitCmd = ReactiveCommand.Create(
            () =>
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                {
                    desktopLifetime.Shutdown();
                }
            }
        ).DisposeWith(Disposables);

        OpenCountriesCmd = ReactiveCommand.Create(
            () =>
            {
                _provider.OpenSingleDock(_provider.GetCountriesViewModel());
            }
        ).DisposeWith(Disposables);

        OpenCitiesCmd = ReactiveCommand.Create(
            async () =>
            {
                if (!await _provider.CountryService.AnyAsync())
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_Countries_Text"]);
                    return;
                }

                _provider.OpenSingleDock(_provider.GetCitiesViewModel());
            }
        ).DisposeWith(Disposables);

        OpenSkillCategoriesCmd = ReactiveCommand.Create(
            () =>
            {
                _provider.OpenSingleDock(_provider.GetCategoriesViewModel());
            }
        ).DisposeWith(Disposables);

        OpenSkillsCmd = ReactiveCommand.Create(
            async () =>
            {
                if (!await _provider.SkillCategoryService.AnyAsync())
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_SkillCategories_Text"]);
                    return;
                }

                _provider.OpenSingleDock(_provider.GetSkillsViewModel());
            }
        ).DisposeWith(Disposables);

        OpenCompaniesCmd = ReactiveCommand.Create(
            () =>
            {
                _provider.OpenSingleDock(_provider.GetCompaniesViewModel());
            }
        ).DisposeWith(Disposables);

        OpenOfficiesCmd = ReactiveCommand.Create(
            async () =>
            {
                if (!(await _provider.CompanyService.AnyAsync() && await _provider.CityService.AnyAsync()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"],
                        LocalizationService.Default["No_CompaniesCities_Text"]);
                    return;
                }

                _provider.OpenSingleDock(_provider.GetOfficiesViewModel());
            }
        ).DisposeWith(Disposables);

        OpenCreateCandidateCmd = ReactiveCommand.Create(
            async () =>
            {
                if (!(await _provider.CityService.AnyAsync() && await _provider.SkillService.AnyAsync()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"],
                        LocalizationService.Default["No_SkillsCities_Text"]);
                    return;
                }

                _provider.OpenDock(_provider.GetCandidateViewModel());
            }
        ).DisposeWith(Disposables);

        OpenSearchCandidateCmd = ReactiveCommand.Create(
            async () =>
            {
                if (!(await _provider.CityService.AnyAsync() && await _provider.SkillService.AnyAsync()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"],
                        LocalizationService.Default["No_SkillsCities_Text"]);
                    return;
                }

                _provider.OpenDock(_provider.GetCandidateSearchViewModel());
            }
        ).DisposeWith(Disposables);

        OpenSearchVacancyCmd = ReactiveCommand.Create(
            async () =>
            {
                if (!(await _provider.OfficeService.AnyAsync() && await _provider.SkillService.AnyAsync()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"],
                        LocalizationService.Default["No_OfficiesSkills_Text"]);
                    return;
                }

                _provider.OpenDock(_provider.GetVacancySearchViewModel());
            }
        ).DisposeWith(Disposables);

        OpenCreateVacancyCmd = ReactiveCommand.Create(
            async () =>
            {
                if (!(await _provider.OfficeService.AnyAsync() && await _provider.SkillService.AnyAsync()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"],
                        LocalizationService.Default["No_OfficiesSkills_Text"]);
                    return;
                }

                _provider.OpenDock(_provider.GetVacancyViewModel());
            }
        ).DisposeWith(Disposables);

        AboutCmd = ReactiveCommand.Create(
            async () =>
            {
                var projectUrl = "https://github.com/poimenov/MyCandidate";
                var standardParams = new MessageBoxStandardParams
                {

                    WindowIcon = MessageBoxExtension.AppLogoIcon,
                    ContentTitle = LocalizationService.Default["About"],
                    ContentMessage = LocalizationService.Default["About_Text"],
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ShowInCenter = true,
                    CanResize = false,
                    ButtonDefinitions = ButtonEnum.Ok,
                    Icon = Icon.Info,
                    HyperLinkParams = new HyperLinkParams
                    {
                        Text = projectUrl,
                        Action = new Action(() =>
                        {
                            DataTemplates.DataTemplateProvider.Open(projectUrl);
                            return;
                        })
                    }
                };
                var messageBox = MessageBoxManager.GetMessageBoxStandard(standardParams);
                await messageBox.ShowAsync();
            }
        ).DisposeWith(Disposables);
    }

    protected override void Dispose(bool disposing)
    {
        LocalizationService.Default.OnCultureChanged -= CultureChanged;
        if (_provider.Documents?.VisibleDockables != null)
        {
            foreach (var dock in _provider.Documents.VisibleDockables)
            {
                var doc = dock as IDisposable;
                if (doc != null)
                {
                    doc.Dispose();
                }
            }
        }
        base.Dispose(disposing);
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["progName"];
        this.RaisePropertyChanged(nameof(Title));
    }

    #region Layout
    private IRootDock? _layout;
    public IRootDock? Layout
    {
        get => _layout;
        set => this.RaiseAndSetIfChanged(ref _layout, value);
    }
    #endregion

    public string Title { get; set; }

    public MenuThemeViewModel MenuThemeViewModel { get; private set; }
    public MenuLanguageViewModel MenuLanguageViewModel { get; private set; }
    public MenuRecentViewModel RecentCandidatesViewModel { get; private set; }
    public MenuRecentViewModel RecentVacanciesViewModel { get; private set; }
    public IReactiveCommand FileExitCmd { get; }
    public IReactiveCommand OpenCountriesCmd { get; }
    public IReactiveCommand OpenCitiesCmd { get; }
    public IReactiveCommand OpenSkillCategoriesCmd { get; }
    public IReactiveCommand OpenSkillsCmd { get; }
    public IReactiveCommand OpenCompaniesCmd { get; }
    public IReactiveCommand OpenOfficiesCmd { get; }
    public IReactiveCommand OpenCreateCandidateCmd { get; }
    public IReactiveCommand OpenSearchCandidateCmd { get; }
    public IReactiveCommand OpenSearchVacancyCmd { get; }
    public IReactiveCommand OpenCreateVacancyCmd { get; }
    public IReactiveCommand AboutCmd { get; }
}
