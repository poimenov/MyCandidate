using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
using log4net;
using MyCandidate.Common;
using MyCandidate.MVVM.Services;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Dictionary;

public class CitiesViewModel : DictionaryViewModel<City>
{
    public CitiesViewModel(IDictionaryService<City> service, ILog log) : base(service, log)
    {
        Id = "Cities";
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        Title = LocalizationService.Default["Cities"];
        var countries = new List<Country>() { new Country() { Id = 0, Name = string.Empty } };
        var countriesList = ItemList.Where(x => x.Country != null).Select(x => x.Country!).Distinct().ToList();
        if (countriesList.Any())
        {
            countries.AddRange(countriesList);
        }

        Countries = countries;
    }

    protected override IObservable<Func<City, bool>>? Filter =>
        this.WhenAnyValue(x => x.Enabled, x => x.Name, x => x.SelectedCountry)
            .Select((x) => MakeFilter(x.Item1, x.Item2, x.Item3));

    private void CultureChanged(object? sender, EventArgs e)
    {
        Title = LocalizationService.Default["Cities"];
    }

    #region Countries
    private IEnumerable<Country>? _countries;
    public IEnumerable<Country>? Countries
    {
        get => _countries;
        set => this.RaiseAndSetIfChanged(ref _countries, value);
    }
    #endregion     

    #region SelectedCountry
    private Country? _selectedCountry;
    public Country? SelectedCountry
    {
        get => _selectedCountry;
        set => this.RaiseAndSetIfChanged(ref _selectedCountry, value);
    }
    #endregion     

    private Func<City, bool> MakeFilter(bool? enabled, string name, Country? country)
    {
        return item =>
        {
            var byCountry = true;
            if (country != null && country.Id != 0)
            {
                byCountry = item.CountryId == country.Id;
            }
            var byName = true;
            if (!string.IsNullOrEmpty(name) && name.Length > 2)
            {
                byName = item.Name.StartsWith(name, true, CultureInfo.InvariantCulture);
            }

            if (enabled.HasValue)
            {
                return item.Enabled == enabled && byName && byCountry;
            }
            else
            {
                return byName && byCountry;
            }
        };
    }
}
