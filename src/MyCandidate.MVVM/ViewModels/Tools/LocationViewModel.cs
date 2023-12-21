using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using PropertyModels.ComponentModel;
using ReactiveUI;

namespace MyCandidate.MVVM.ViewModels.Tools;

public class LocationViewModel : ViewModelBase
{
    private bool _locationChanges = false;
    public LocationViewModel(IDataAccess<Country> countries, IDataAccess<City> cities)
    {
        Countries = countries.ItemsList.Where(x => x.Enabled == true);
        CitiesSource = new ObservableCollectionExtended<City>(cities!.ItemsList.Where(x => x.Enabled == true));
        CitiesSource.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(Filter)
            .Bind(out _citiesList)
            .Subscribe();

        this.WhenAnyValue(x => x.Location)
            .Subscribe
            (
                x =>
                {
                    if (x != null && !_locationChanges)
                    {
                        _locationChanges = true;
                        var countryId = Cities.First(c => c.Id == x.CityId).CountryId;
                        Country = Countries.First(c => c.Id == countryId);
                        City = Cities.First(c => c.Id == x.CityId);
                        Address = x.Address;
                        _locationChanges = false;
                    }
                }
            );

        this.WhenAnyValue(x => x.Address)
            .Subscribe
            (
                x =>
                {
                    if (x != null && Location.Address != x)
                    {
                        Location.Address = x;
                        this.RaisePropertyChanged(nameof(this.Location));
                    }
                }
            );

        this.WhenAnyValue(x => x.City)
            .Subscribe
            (
                x =>
                {
                    if (x != null && Location.CityId != x.Id)
                    {
                        Location.CityId = x.Id;
                        Location.City = x;
                        this.RaisePropertyChanged(nameof(this.Location));
                    }
                }
            );

        this.WhenAnyValue(x => x.Country)
            .Subscribe
            (
                x =>
                {
                    if (x != null && Cities.Any(c => c.CountryId == x.Id) && !_locationChanges)
                    {
                        City = Cities.First(c => c.CountryId == x.Id);
                    }
                }
            );
    }
    public ObservableCollectionExtended<City> CitiesSource;
    private readonly ReadOnlyObservableCollection<City> _citiesList;
    public ReadOnlyObservableCollection<City> Cities => _citiesList;

    private IEnumerable<Country> _countriesList;
    public IEnumerable<Country> Countries
    {
        get => _countriesList;
        set => _countriesList = value;
    }

    private Location _location;
    public Location Location
    {
        get => _location;
        set => this.RaiseAndSetIfChanged(ref _location, value);
    }

    private Country _country;
    public Country Country
    {
        get => _country;
        set => this.RaiseAndSetIfChanged(ref _country, value);
    }

    private City _city;
    public City City
    {
        get => _city;
        set => this.RaiseAndSetIfChanged(ref _city, value);
    }

    private String _address;
    //[Required]
    [StringLength(250)]
    public String Address
    {
        get => _address;
        set => this.RaiseAndSetIfChanged(ref _address, value);
    }

    private IObservable<Func<City, bool>>? Filter =>
        this.WhenAnyValue(x => x.Country)
            .Select((x) => MakeFilter(x));

    private Func<City, bool> MakeFilter(Country? country)
    {
        return item =>
        {
            var retVal = true;
            if (country != null && country.Id != 0)
            {
                retVal = item.CountryId == country.Id;
            }

            return retVal;
        };
    }
}
