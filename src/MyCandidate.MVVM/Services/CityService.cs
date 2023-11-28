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

    public void Create(IEnumerable<City> items)
    {
        _cities.Create(items);
    }

    public void Delete(IEnumerable<int> itemIds)
    {
        _cities.Delete(itemIds);
    }

    public void Update(IEnumerable<City> items)
    {
        _cities.Update(items);
    }
}
