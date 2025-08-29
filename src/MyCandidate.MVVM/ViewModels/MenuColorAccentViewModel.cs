using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class MenuColorAccentViewModel : CheckMenuModel
{
    public ReadOnlyObservableCollection<MenuItem> Items { get; private set; }
    public ReactiveCommand<string, Task> ChangeColorAccentCmd { get; }

    public MenuColorAccentViewModel()
    {
        var _appSettings = GetAppSettings();
        var defaultTheme = _appSettings.DefaultTheme;
        var paletteName = _appSettings.Palette;
        App.ThemeManager?.Switch(defaultTheme, paletteName);

        ChangeColorAccentCmd = ReactiveCommand.Create<string, Task>(
            async (palette) =>
            {
                var appSettings = GetAppSettings();
                App.ThemeManager?.Switch(appSettings.DefaultTheme, palette);
                appSettings.Palette = palette;
                await appSettings.SaveAsync();
                foreach (var item in Items!)
                {
                    if (Exists(item)
                        && item.CommandParameter is string
                        && (string)item.CommandParameter == palette)
                    {
                        item.Icon = CheckImage;
                    }
                    else
                    {
                        item.Icon = null;
                    }
                }
            }).DisposeWith(Disposables);


        var items = new List<MenuItem>();
        var themesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes");
        if (Directory.Exists(themesFolderPath))
        {
            var themesDirectory = new DirectoryInfo(themesFolderPath);
            items.AddRange(themesDirectory
                .GetFiles("*.axaml")
                .Select(x => Path.GetFileNameWithoutExtension(x.Name))
                .Select(x => GetMenuItem(x, paletteName)));
        }

        Items = new ReadOnlyObservableCollection<MenuItem>(
            new ObservableCollection<MenuItem>(items));

    }
    private MenuItem GetMenuItem(string palette, string? defaultPalette)
    {
        var retVal = new MenuItem();
        retVal.Header = palette;
        retVal.Tag = palette;
        retVal.Command = ChangeColorAccentCmd;
        retVal.CommandParameter = palette;
        if (string.Equals(palette, defaultPalette, StringComparison.OrdinalIgnoreCase))
        {
            retVal.Icon = CheckImage;
        }
        return retVal;
    }

    private bool Exists(MenuItem? item)
    {
        var palette = Convert.ToString(item?.Tag);
        return !string.IsNullOrWhiteSpace(palette)
            && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Themes",
                $"{palette}.axaml"));
    }
}

