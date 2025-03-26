using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Services;
using MyCandidate.Common;
using PropertyModels.Localization;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels;

public class MenuLanguageViewModel : CheckMenuModel
{
    public ReadOnlyObservableCollection<MenuItem> Items { get; private set; }
    public ReactiveCommand<ICultureData, Unit> ChangeLanguageCmd { get; }

    public MenuLanguageViewModel(AppSettings appSettings) : base(appSettings)
    {
        var allCultures = LocalizationService.Default.GetCultures();
        var defaultCulture = allCultures.FirstOrDefault(x => x.Culture.Name == _appSettings.DefaultLanguage);
        if (defaultCulture == null)
        {
            defaultCulture = allCultures.FirstOrDefault() ?? throw new InvalidOperationException("No cultures available");
        }

        LocalizationService.Default.SelectCulture(defaultCulture.Culture.Name);

        ChangeLanguageCmd = ReactiveCommand.Create<ICultureData, Unit>(
            (lang) =>
            {
                LocalizationService.Default.SelectCulture(lang.Culture.Name);
                _appSettings.DefaultLanguage = lang.Culture.Name;
                _appSettings.Save();
                foreach (var item in Items!)
                {
                    if (item.CommandParameter != null
                        && item.CommandParameter is ICultureData
                        && (ICultureData)item.CommandParameter == lang)
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
            new ObservableCollection<MenuItem>(
                allCultures.Select(x => GetMenuItem(x, defaultCulture))
            )
        );

    }    private MenuItem GetMenuItem(ICultureData culture, ICultureData defaultCulture)    {
        var retVal = new MenuItem();
        retVal.Header = culture;
        retVal.Tag = culture;
        retVal.Command = ChangeLanguageCmd;
        retVal.CommandParameter = culture;
        if (culture == defaultCulture)
        {
            retVal.Icon = CheckImage;
        }
        return retVal;
    }
}
