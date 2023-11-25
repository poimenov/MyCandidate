using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using MyCandidate.Common;
using MyCandidate.MVVM.Themes;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class MenuThemeViewModel : CheckMenuModel
{
    public ReadOnlyObservableCollection<MenuItem> Items { get; private set; }
    public ReactiveCommand<ThemeName, Unit> ChangeThemeCmd { get; }

    public MenuThemeViewModel(AppSettings appSettings) : base(appSettings)
    {
        ThemeName defaultTheme = ThemeName.Light;
        Enum.TryParse<ThemeName>(_appSettings.DefaultTheme, out defaultTheme);
        App.ThemeManager?.Switch(defaultTheme);

        ChangeThemeCmd = ReactiveCommand.Create<ThemeName, Unit>(
            (theme) =>
            {
                App.ThemeManager?.Switch(theme);
                _appSettings.DefaultTheme = theme.ToString();
                _appSettings.Save();
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
                return Unit.Default;
            });

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
