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
        var documents = _factory?.GetDockable<IDocumentDock>("Documents");
        if (documents?.VisibleDockables != null)
        {
            var doc = documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(CountriesViewModel));
            OpenViewModel(doc ?? CountriesViewModel, doc == null);
        }
    }

    public void OpenCities()
    {
        var documents = _factory?.GetDockable<IDocumentDock>("Documents");
        if (documents?.VisibleDockables != null)
        {
            var doc = documents.VisibleDockables.FirstOrDefault(x => x.GetType() == typeof(CitiesViewModel));
            OpenViewModel(doc ?? CitiesViewModel, doc == null);
        }
    }

    private void OpenViewModel(IDockable dockable, bool addNew)
    {
        var documents = _factory?.GetDockable<IDocumentDock>("Documents");
        if (Layout is { } && documents is { })
        {
            if (addNew)
            {
                _factory?.AddDockable(documents, dockable);
            }

            _factory?.SetActiveDockable(dockable);
            _factory?.SetFocusedDockable(Layout, dockable);
        }
    }

    #region CountriesViewModel
    private DictionaryViewModel<Country>? _countriesViewModel;
    public DictionaryViewModel<Country>? CountriesViewModel
    {
        get
        {
            if (_countriesViewModel == null)
            {
                _countriesViewModel = ((App)Application.Current).CountriesViewModel;
                _countriesViewModel!.Properties = _factory?.GetDockable<PropertiesViewModel>("Properties") as IProperties;
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
                _citiesViewModel!.Properties = _factory?.GetDockable<PropertiesViewModel>("Properties") as IProperties;
            }
            return _citiesViewModel;
        }
        set
        {
            _citiesViewModel = value;
        }
    }
    #endregion    
}
