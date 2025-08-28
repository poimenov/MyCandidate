using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using MyCandidate.Common;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class MenuThemeViewModel : CheckMenuModel
{
    public ReadOnlyObservableCollection<MenuItem> Items { get; private set; }
    public ReactiveCommand<ThemeName, Task> ChangeThemeCmd { get; }

    public MenuThemeViewModel(AppSettings appSettings) : base(appSettings)
    {
        var defaultTheme = _appSettings.DefaultTheme;
        var paletteName = _appSettings.Palette;
        App.ThemeManager?.Switch(defaultTheme, paletteName);

        ChangeThemeCmd = ReactiveCommand.Create<ThemeName, Task>(
            async (theme) =>
            {
                App.ThemeManager?.Switch(theme, paletteName);
                _appSettings.DefaultTheme = theme;
                await _appSettings.SaveAsync();
                foreach (var item in Items!)
                {
                    if (item.CommandParameter != null
                        && item.CommandParameter is ThemeName
                        && (ThemeName)item.CommandParameter == theme)
                    {
                        item.Icon = CheckImage;
                    }
                    else
                    {
                        item.Icon = null;
                    }
                }
            }).DisposeWith(Disposables);

        Items = new ReadOnlyObservableCollection<MenuItem>(
            new ObservableCollection<MenuItem>
            {
                GetMenuItem(ThemeName.Dark, defaultTheme),
                GetMenuItem(ThemeName.Light, defaultTheme)
            });

    }
    private MenuItem GetMenuItem(ThemeName themeName, ThemeName defaultTheme)
    {
        var retVal = new MenuItem();
        retVal.Header = themeName;
        retVal.Tag = themeName;
        retVal.Command = ChangeThemeCmd;
        retVal.CommandParameter = themeName;
        if (themeName == defaultTheme)
        {
            retVal.Icon = CheckImage;
        }
        return retVal;
    }
}
