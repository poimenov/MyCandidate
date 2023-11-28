using System;
using System.Collections.Generic;
using System.Linq;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class CountryService : IDictionaryService<Country>
{
    private readonly IDataAccess<Country> _countries;
    private readonly IDataAccess<City> _cities;
    public CountryService(IDataAccess<Country> countries, IDataAccess<City> cities)
    {
        _countries = countries;
        _cities = cities;
    }

    public IEnumerable<Country> ItemsList => _countries.ItemsList;

    public void Create(IEnumerable<Country> items)
    {
        _countries.Create(items);
    }

    public void Delete(IEnumerable<int> itemIds)
    {
        if (_cities.ItemsList.Any(x => itemIds.Contains(x.CountryId)))
        {
            var countryIds = _cities.ItemsList.Where(x => itemIds.Contains(x.CountryId)).Select(x => x.CountryId).ToList();
            var countryNames = _countries.ItemsList.Where(x => countryIds.Contains(x.Id)).Select(x => $"\"{x.Name}\"").ToArray();

            throw new Exception($"It is impossible to delete next countries: {string.Join(", ", countryNames)} because there are cities associated with it");
        }

        _countries.Delete(itemIds);
    }

    public void Update(IEnumerable<Country> items)
    {
        _countries.Update(items);
    }
}
