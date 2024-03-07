using System;
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
        MenuThemeViewModel = new MenuThemeViewModel(_options.Value);
        MenuLanguageViewModel = new MenuLanguageViewModel(_options.Value);
        RecentCandidatesViewModel = new MenuRecentViewModel(_provider, Models.TargetModelType.Candidate, 10);
        RecentVacanciesViewModel = new MenuRecentViewModel(_provider, Models.TargetModelType.Vacancy, 5);

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
        );

        OpenCountriesCmd = ReactiveCommand.Create(
            () =>
            {
                _provider.OpenSingleDock(_provider.GetCountriesViewModel());
            }
        );

        OpenCitiesCmd = ReactiveCommand.Create(
            () =>
            {
                if (!_provider.CountryService.Any())
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_Countries_Text"]);
                    return;
                }

                _provider.OpenSingleDock(_provider.GetCitiesViewModel());
            }
        );

        OpenSkillCategoriesCmd = ReactiveCommand.Create(
            () =>
            {
                _provider.OpenSingleDock(_provider.GetCategoriesViewModel());
            }
        );

        OpenSkillsCmd = ReactiveCommand.Create(
            () =>
            {
                if (!_provider.SkillCategoryService.Any())
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_SkillCategories_Text"]);
                    return;
                } 

                _provider.OpenSingleDock(_provider.GetSkillsViewModel());
            }
        );

        OpenCompaniesCmd = ReactiveCommand.Create(
            () =>
            {
                _provider.OpenSingleDock(_provider.GetCompaniesViewModel());
            }
        );

        OpenOfficiesCmd = ReactiveCommand.Create(
            () =>
            {
                if (!(_provider.CompanyService.Any() && _provider.CityService.Any()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_CompaniesCities_Text"]);
                    return;
                } 

                _provider.OpenSingleDock(_provider.GetOfficiesViewModel());
            }
        );

        OpenCreateCandidateCmd = ReactiveCommand.Create(
            () =>
            {
                if (!(_provider.CityService.Any() && _provider.SkillService.Any()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_SkillsCities_Text"]);
                    return;
                } 

                _provider.OpenDock(_provider.GetCandidateViewModel());
            }
        );

        OpenSearchCandidateCmd = ReactiveCommand.Create(
            () =>
            {
                if (!(_provider.CityService.Any() && _provider.SkillService.Any()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_SkillsCities_Text"]);
                    return;
                } 

                _provider.OpenDock(_provider.GetCandidateSearchViewModel());
            }
        );

        OpenSearchVacancyCmd = ReactiveCommand.Create(
            () =>
            {
                if (!(_provider.OfficeService.Any() && _provider.SkillService.Any()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_OfficiesSkills_Text"]);
                    return;
                } 

                _provider.OpenDock(_provider.GetVacancySearchViewModel());
            }
        );

        OpenCreateVacancyCmd = ReactiveCommand.Create(
            () =>
            {
                if (!(_provider.OfficeService.Any() && _provider.SkillService.Any()))
                {
                    ShowMessageBox(LocalizationService.Default["CommandIsUnawailable"], LocalizationService.Default["No_OfficiesSkills_Text"]);
                    return;
                } 

                _provider.OpenDock(_provider.GetVacancyViewModel());
            }
        );

        AboutCmd = ReactiveCommand.Create(
            () =>
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
                messageBox.ShowAsync();
            }
        );
    }

    #region Layout
    private IRootDock? _layout;
    public IRootDock? Layout
    {
        get => _layout;
        set => this.RaiseAndSetIfChanged(ref _layout, value);
    }
    #endregion

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
