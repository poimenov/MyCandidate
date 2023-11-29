using System.Collections.Generic;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class CityService : IDictionaryService<City>
{
    private readonly IDataAccess<City> _cities;
    public CityService(IDataAccess<City> cities)
    {
        _cities = cities;
    }
    
    public IEnumerable<City> ItemsList => _cities.ItemsList;

    public bool Create(IEnumerable<City> items, out string message)
    {
        message = string.Empty;
        _cities.Create(items);
        return true;
    }

    public bool Delete(IEnumerable<int> itemIds, out string message)
    {
        message = string.Empty;
        _cities.Delete(itemIds);
        return true;
    }

    public bool Update(IEnumerable<City> items, out string message)
    {
        message = string.Empty;
        _cities.Update(items);
        return true;
    }
}
