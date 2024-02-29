using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class CityService : IDictionaryService<City>
{
    private readonly IDataAccess<City> _cities;
    private readonly ILog _log;
    public CityService(IDataAccess<City> cities, ILog log)
    {
        _cities = cities;
        _log = log;
    }

    public IEnumerable<City> ItemsList => _cities.ItemsList;

    public bool Create(IEnumerable<City> items, out string message)
    {
        message = string.Empty;
        IEnumerable<string> names;
        if (CheckDuplicates(items, out names))
        {
            message = $"It is impossible to add next cities: {string.Join(", ", names)} because they already exist";
            return false;
        }

        try
        {
            _cities.Create(items);
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
        _cities.Delete(itemIds);
        return true;
    }

    public City Get(int id)
    {
        return _cities.Get(id);
    }

    public bool Update(IEnumerable<City> items, out string message)
    {
        message = string.Empty;
        IEnumerable<string> names;
        if (CheckDuplicates(items, out names))
        {
            message = $"It is impossible to update next cities: {string.Join(", ", names)} because they already exist";
            return false;
        }

        try
        {
            _cities.Update(items);
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
        return _cities.Any();
    }

    private bool CheckDuplicates(IEnumerable<City> items, out IEnumerable<string> names)
    {
        names = Enumerable.Empty<string>();
        var existedItems = ItemsList.Select(x => new KeyValuePair<int, string>(x.CountryId, x.Name.Trim().ToLower()));
        var newItems = items.Select(x => new KeyValuePair<int, string>(x.CountryId, x.Name.Trim().ToLower()));
        var allItems = new List<KeyValuePair<int, string>>();
        allItems.AddRange(existedItems);
        allItems.AddRange(newItems);
        var duplicates = allItems.Select(x => new { CountryId = x.Key, Name = x.Value })
            .GroupBy(x => new { x.CountryId, x.Name }).Where(x => x.Count() > 1);
        if(duplicates.Any())
        {
            names = duplicates.Select(x => x.Key.Name);
            return true;
        }

        return false;
    }
}
