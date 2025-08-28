using System;
using System.IO;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using MyCandidate.Common;

namespace MyCandidate.MVVM.Themes;

public class FluentThemeManager : IThemeManager
{
    private static readonly Uri BaseUri = new("avares://MyCandidate.MVVM/Themes");

    private static readonly IStyle Default = new FluentTheme()
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

    private static IStyle LoadThemeFromFile(string? paletteName)
    {
        if (string.IsNullOrWhiteSpace(paletteName))
            return Default;

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes", $"{paletteName}.axaml");
        if (!File.Exists(filePath))
            return Default;

        try
        {
            string xamlContent = File.ReadAllText(filePath);
            var retVal = AvaloniaRuntimeXamlLoader.Parse<IStyle>(xamlContent);
            return retVal ?? Default;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading theme: {ex.Message}");
            return Default;
        }
    }

    public void Switch(ThemeName themeName, string? paletteName)
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
                    Application.Current.Styles[0] = LoadThemeFromFile(paletteName);
                    Application.Current.Styles[1] = DockFluent;
                    Application.Current.Styles[2] = DataGridFluent;
                    Application.Current.Styles[3] = FluentLight;
                    break;
                }
            case ThemeName.Dark:
                {
                    Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                    Application.Current.Styles[0] = LoadThemeFromFile(paletteName);
                    Application.Current.Styles[1] = DockFluent;
                    Application.Current.Styles[2] = DataGridFluent;
                    Application.Current.Styles[3] = FluentDark;
                    break;
                }
        }
    }

    public void Initialize(Application application)
    {
        application.Resources.MergedDictionaries.Insert(0, ResourceTheme);
        application.Styles.Insert(0, Default);
        application.Styles.Insert(1, DockFluent);
        application.Styles.Insert(2, DataGridFluent);
        application.Styles.Insert(3, FluentLight);
        application.Styles.Insert(4, GroupBoxClassic);
        application.Styles.Insert(5, Hyperlink);
        application.Styles.Insert(6, DockableHeader);
    }
}

