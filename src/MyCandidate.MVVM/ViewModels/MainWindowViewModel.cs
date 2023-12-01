using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.PropertyGrid.Services;
using Dock.Model.Controls;
using Dock.Model.Core;
using Microsoft.Extensions.Options;
using MyCandidate.Common;
using MyCandidate.MVVM.Localizations;
using MyCandidate.MVVM.ViewModels.Dictionary;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IOptions<AppSettings> _options;

    private readonly IFactory? _factory;
    private IRootDock? _layout;
    public MenuThemeViewModel MenuThemeViewModel { get; private set; }
    public MenuLanguageViewModel MenuLanguageViewModel { get; private set; }

    public IRootDock? Layout
    {
        get => _layout;
        set => this.RaiseAndSetIfChanged(ref _layout, value);
    }

    public MainWindowViewModel(IOptions<AppSettings> options)
    {
        _options = options;
        LocalizationService.Default.AddExtraService(new AppLocalizationService());
        this.MenuThemeViewModel = new MenuThemeViewModel(_options.Value);
        this.MenuLanguageViewModel = new MenuLanguageViewModel(_options.Value);
        _factory = new DockFactory();
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

    private void OpenViewModel(IDockable dockable, bool addNew)
    {
        if (Layout is { } && Documents is { })
        {
            if (addNew)
            {
                _factory?.AddDockable(Documents, dockable);
            }

            _factory?.SetActiveDockable(dockable);
            _factory?.SetFocusedDockable(Layout, dockable);
        }
    }

    #region Documents
    private IDocumentDock? _documents;
    public IDocumentDock? Documents
    {
        get
        {
            if (_documents == null)
            {
                _documents = _factory?.GetDockable<IDocumentDock>("Documents");
            }

            return _documents;
        }
    }
    #endregion

    #region Properties
    private IProperties? _properties;
    public IProperties? Properties
    {
        get
        {
            if (_properties == null)
            {
                _properties = _factory?.GetDockable<PropertiesViewModel>("Properties") as IProperties;
            }

            return _properties;
        }
    }
    #endregion

    #region CountriesViewModel
    private DictionaryViewModel<Country>? _countriesViewModel;
    public DictionaryViewModel<Country>? CountriesViewModel
    {
        get
        {
            if (_countriesViewModel == null)
            {
                _countriesViewModel = ((App)Application.Current).CountriesViewModel;
                _countriesViewModel!.Properties = Properties;
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
                _citiesViewModel = ((App)Application.Current).CitiesViewModel;
                _citiesViewModel!.Properties = Properties;
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
                _categoriesViewModel = ((App)Application.Current).CategoriesViewModel;
                _categoriesViewModel!.Properties = Properties;
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
                _skillsViewModel = ((App)Application.Current).SkillsViewModel;
                _skillsViewModel!.Properties = Properties;
            }
            return _skillsViewModel;
        }
        set
        {
            _skillsViewModel = value;
        }
    }
    #endregion          
}
