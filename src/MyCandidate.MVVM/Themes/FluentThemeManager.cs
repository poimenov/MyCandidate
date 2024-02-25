using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace MyCandidate.MVVM.Themes;

public class FluentThemeManager : IThemeManager
{
    private static readonly Uri BaseUri = new("avares://MyCandidate.MVVM/Themes");

    private static readonly FluentTheme Fluent = new()
    {
    };

    private static readonly Styles DockFluent = new()
    {
        new StyleInclude(BaseUri)
        {
            Source = new Uri("avares://Dock.Avalonia/Themes/DockFluentTheme.axaml")
        }
    };

    private static readonly Styles DataGridFluent = new()
    {
        new StyleInclude(BaseUri)
        {
            Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml")
        }
    };

    private static readonly Styles FluentDark = new()
    {
        new StyleInclude(BaseUri)
        {
            Source = new Uri("avares://MyCandidate.MVVM/Themes/FluentDark.axaml")
        }
    };

    private static readonly Styles FluentLight = new()
    {
        new StyleInclude(BaseUri)
        {
            Source = new Uri("avares://MyCandidate.MVVM/Themes/FluentLight.axaml")
        }
    };

    private static readonly Styles GroupBoxClassic = new()
    {
        new StyleInclude(BaseUri)
        {
            Source = new Uri("avares://MyCandidate.MVVM/Themes/GroupBoxClassic.axaml")
        }
    };  

    private static readonly Styles DockableHeader = new()
    {
        new StyleInclude(BaseUri)
        {
            Source = new Uri("avares://MyCandidate.MVVM/Themes/DockableHeader.axaml")
        }
    };       

    private static readonly Styles Hyperlink = new()
    {
        new StyleInclude(BaseUri)
        {
            Source = new Uri("avares://MyCandidate.MVVM/Themes/Hyperlink.axaml")
        }
    };      

    private static readonly MergeResourceInclude ResourceTheme = new(BaseUri)
    {
        Source = new Uri("avares://MyCandidate.MVVM/Themes/Themes.axaml")
    };

    public void Switch(ThemeName themeName)
    {
        if (Application.Current is null)
        {
            return;
        }

        switch (themeName)
        {
            case ThemeName.Light:
                {
                    Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                    Application.Current.Styles[0] = Fluent;
                    Application.Current.Styles[1] = DockFluent;
                    Application.Current.Styles[2] = DataGridFluent;
                    Application.Current.Styles[3] = FluentLight;
                    break;
                }
            case ThemeName.Dark:
                {
                    Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                    Application.Current.Styles[0] = Fluent;
                    Application.Current.Styles[1] = DockFluent;
                    Application.Current.Styles[2] = DataGridFluent;
                    Application.Current.Styles[3] = FluentDark;
                    break;
                }
        }
    }

    public void Initialize(Application application)
    {
        application.Resources.MergedDictionaries.Insert(0,ResourceTheme);
        application.Styles.Insert(0, Fluent);
        application.Styles.Insert(1, DockFluent);
        application.Styles.Insert(2, DataGridFluent);
        application.Styles.Insert(3, FluentLight);
        application.Styles.Insert(4, GroupBoxClassic);
        application.Styles.Insert(5, Hyperlink);
        application.Styles.Insert(6, DockableHeader);
    }
}

