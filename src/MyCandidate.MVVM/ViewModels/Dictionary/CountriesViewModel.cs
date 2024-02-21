using System;
using Avalonia.PropertyGrid.Services;
using log4net;
using MyCandidate.Common;
using MyCandidate.MVVM.Services;

namespace MyCandidate.MVVM.ViewModels.Dictionary;

public class CountriesViewModel : DictionaryViewModel<Country>
{
    public CountriesViewModel(IDictionaryService<Country> service, ILog log) : base(service, log)
    {
        Id = "Countries";
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        Title = LocalizationService.Default["Countries"];
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["Countries"];
    }    
}
