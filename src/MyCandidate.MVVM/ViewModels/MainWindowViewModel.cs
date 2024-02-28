using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.PropertyGrid.Services;
using Dock.Model.Controls;
using Dock.Model.Core;
using Microsoft.Extensions.Options;
using MyCandidate.Common;
using MyCandidate.MVVM.Localizations;
using MyCandidate.MVVM.Services;
using MyCandidate.MVVM.ViewModels.Dictionary;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAppServiceProvider _appServiceProvider;    
    private readonly IOptions<AppSettings> _options;
    private readonly IFactory? _factory;
    private IRootDock? _layout;
    public MenuThemeViewModel MenuThemeViewModel { get; private set; }
    public MenuLanguageViewModel MenuLanguageViewModel { get; private set; }  
    public MenuRecentViewModel RecentCandidatesViewModel { get; private set; } 
    public MenuRecentViewModel RecentVacanciesViewModel { get; private set; } 

    public IRootDock? Layout
    {
        get => _layout;
        set => this.RaiseAndSetIfChanged(ref _layout, value);
    }

    public MainWindowViewModel(IAppServiceProvider appServiceProvider, IOptions<AppSettings> options)
    {
        _appServiceProvider = appServiceProvider;
        _options = options;
        LocalizationService.Default.AddExtraService(new AppLocalizationService());
        MenuThemeViewModel = new MenuThemeViewModel(_options.Value);
        MenuLanguageViewModel = new MenuLanguageViewModel(_options.Value);
        RecentCandidatesViewModel = new MenuRecentViewModel(_appServiceProvider, Models.TargetModelType.Candidate, 10);
        RecentVacanciesViewModel = new MenuRecentViewModel(_appServiceProvider, Models.TargetModelType.Vacancy, 5);
        _factory = appServiceProvider.Factory;
        Layout = _factory?.CreateLayout();
        if (Layout is { })
        {
            _factory?.InitLayout(Layout);
        }
    }

    public void FileExit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.Shutdown();
        }
    }

    public void OpenCountries()
    {
        if (Documents?.VisibleDockables != null)
        {
            var doc = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(CountriesViewModel));
            OpenViewModel(doc ?? CountriesViewModel, doc == null);
        }
    }

    public void OpenCities()
    {
        if (Documents?.VisibleDockables != null)
        {
            var doc = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(CitiesViewModel));
            OpenViewModel(doc ?? CitiesViewModel, doc == null);
        }
    }

    public void OpenSkillCategories()
    {
        if (Documents?.VisibleDockables != null)
        {
            var doc = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(CategoriesViewModel));
            OpenViewModel(doc ?? CategoriesViewModel, doc == null);
        }
    }

    public void OpenSkills()
    {
        if (Documents?.VisibleDockables != null)
        {
            var doc = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(SkillsViewModel));
            OpenViewModel(doc ?? SkillsViewModel, doc == null);
        }
    }

    public void OpenCompanies()
    {
        if (Documents?.VisibleDockables != null)
        {
            var doc = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(CompaniesViewModel));
            OpenViewModel(doc ?? CompaniesViewModel, doc == null);
        }
    } 
    
    public void OpenOfficies()
    {
        if (Documents?.VisibleDockables != null)
        {
            var doc = Documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(OfficiesViewModel));
            OpenViewModel(doc ?? OfficiesViewModel, doc == null);
        }
    }   

    public void OpenCreateCandidate()
    {
        _appServiceProvider.OpenDock(_appServiceProvider.GetCandidateViewModel());
    } 

    public void OpenSearchCandidate()
    {
        _appServiceProvider.OpenDock(_appServiceProvider.GetCandidateSearchViewModel());
    }

    public void OpenSearchVacancy()
    {
        _appServiceProvider.OpenDock(_appServiceProvider.GetVacancySearchViewModel());
    }    
    
    public void OpenCreateVacancy()
    {
        _appServiceProvider.OpenDock(_appServiceProvider.GetVacancyViewModel());
    }         

    private void OpenViewModel(IDockable dockable, bool addNew)
    {
        if (Layout is { } && Documents is { })
        {
            if (addNew)
            {
                _appServiceProvider.OpenDock(dockable);
            }

            _factory?.SetActiveDockable(dockable);
            _factory?.SetFocusedDockable(Layout, dockable);
        }
    }

    public IDocumentDock? Documents => _appServiceProvider.Documents;
    public IProperties? Properties => _appServiceProvider.Properties;

    #region CountriesViewModel
    private DictionaryViewModel<Country>? _countriesViewModel;
    public DictionaryViewModel<Country>? CountriesViewModel
    {
        get
        {
            if (_countriesViewModel == null)
            {
                _countriesViewModel = _appServiceProvider.GetCountriesViewModel();
            }
            return _countriesViewModel;
        }
        set
        {
            _countriesViewModel = value;
        }
    }
    #endregion

    #region CitiesViewModel
    private DictionaryViewModel<City>? _citiesViewModel;
    public DictionaryViewModel<City>? CitiesViewModel
    {
        get
        {
            if (_citiesViewModel == null)
            {
                _citiesViewModel = _appServiceProvider.GetCitiesViewModel();
            }
            return _citiesViewModel;
        }
        set
        {
            _citiesViewModel = value;
        }
    }
    #endregion  

    #region CountriesViewModel
    private DictionaryViewModel<SkillCategory>? _categoriesViewModel;
    public DictionaryViewModel<SkillCategory>? CategoriesViewModel
    {
        get
        {
            if (_categoriesViewModel == null)
            {
                _categoriesViewModel = _appServiceProvider.GetCategoriesViewModel();
            }
            return _categoriesViewModel;
        }
        set
        {
            _categoriesViewModel = value;
        }
    }
    #endregion  

    #region CitiesViewModel
    private DictionaryViewModel<Skill>? _skillsViewModel;
    public DictionaryViewModel<Skill>? SkillsViewModel
    {
        get
        {
            if (_skillsViewModel == null)
            {
                _skillsViewModel = _appServiceProvider.GetSkillsViewModel();
            }
            return _skillsViewModel;
        }
        set
        {
            _skillsViewModel = value;
        }
    }
    #endregion  

    #region CompaniesViewModel
    private DictionaryViewModel<Company>? _companiesViewModel;
    public DictionaryViewModel<Company>? CompaniesViewModel
    {
        get
        {
            if (_companiesViewModel == null)
            {
                _companiesViewModel = _appServiceProvider.GetCompaniesViewModel();
            }
            return _companiesViewModel;
        }
        set
        {
            _companiesViewModel = value;
        }
    }
    #endregion        

    #region OfficiesViewModel
    private DictionaryViewModel<Office>? _officiesViewModel;
    public DictionaryViewModel<Office>? OfficiesViewModel
    {
        get
        {
            if (_officiesViewModel == null)
            {
                _officiesViewModel = _appServiceProvider.GetOfficiesViewModel();
            }
            return _officiesViewModel;
        }
        set
        {
            _officiesViewModel = value;
        }
    }
    #endregion           
}
