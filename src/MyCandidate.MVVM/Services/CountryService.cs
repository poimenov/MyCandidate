using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class CountryService : IDictionaryService<Country>
{
    private readonly IDataAccess<Country> _countries;
    private readonly IDataAccess<City> _cities;
    private readonly ILog _log;
    public CountryService(IDataAccess<Country> countries, IDataAccess<City> cities, ILog log)
    {
        _countries = countries;
        _cities = cities;
        _log = log;
    }

    public IEnumerable<Country> ItemsList => _countries.ItemsList;

    public bool Create(IEnumerable<Country> items, out string message)
    {
        message = string.Empty;
        var itemList = ItemsList;
        if (itemList.Any(x => items.Select(y => y.Name).Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)))
        {
            var countryNames = items
                .Where(x => itemList.Select(y => y.Name)
                .Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
                .Select(x => $"\"{x.Name}\"");
            message = $"It is impossible to add next countries: {string.Join(", ", countryNames)} because they already exist";
            return false;
        }

        try
        {
            _countries.Create(items);
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }

    public bool Delete(IEnumerable<int> itemIds, out string message)
    {
        message = string.Empty;
        var citiesList = _cities.ItemsList;
        if (citiesList.Any(x => itemIds.Contains(x.CountryId)))
        {
            var countryIds = citiesList
                                .Where(x => itemIds.Contains(x.CountryId))
                                .Select(x => x.CountryId);
            var countryNames = _countries.ItemsList
                                            .Where(x => countryIds.Contains(x.Id))
                                            .Select(x => $"\"{x.Name}\"");

            message = $"It is impossible to delete next countries: {string.Join(", ", countryNames)} because there are cities associated with it";
            return false;
        }

        try
        {
            _countries.Delete(itemIds);
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }

    public Country? Get(int id)
    {
        return _countries.Get(id);
    }

    public bool Update(IEnumerable<Country> items, out string message)
    {
        message = string.Empty;

        var originalItems = _countries.ItemsList;
        foreach(var item in items)
        {
            var existedItem = originalItems.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase) && x.Id != item.Id);            
            if(existedItem != null)
            {
                message = $"It is impossible to update next country: {existedItem.Name} because a country with that name already exists";
                return false;
            }

            var newItem = items.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase) && x.Id != item.Id); 
            if(newItem != null)
            {
                message = $"Same name for country: {newItem.Name}";
                return false;
            }                       
        }

        try
        {
            _countries.Update(items);
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }

    public bool Any()
    {
        return _countries.Any();
    }
}
