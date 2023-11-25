using System;
using Avalonia.PropertyGrid.Services;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.ViewModels.Dictionary;

public class CountriesViewModel : DictionaryViewModel<Country>
{
    public CountriesViewModel(IDataAccess<Country> dataAccess, ILog log) : base(dataAccess, log)
    {
        Id = "Countries";
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        Title = LocalizationService.Default["Countries"];
        SelectedTypeName = LocalizationService.Default["Country"];
    }

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["Countries"];
        SelectedTypeName = LocalizationService.Default["Country"];
    }    
}
