using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<IEnumerable<Country>> GetItemsListAsync()
    {
        return await _countries.GetItemsListAsync();
    }

    public async Task<OperationResult> CreateAsync(IEnumerable<Country> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            var itemList = await _countries.GetItemsListAsync();
            if (itemList.Any(x => items.Select(y => y.Name).Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)))
            {
                var countryNames = items
                    .Where(x => itemList.Select(y => y.Name)
                    .Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
                    .Select(x => $"\"{x.Name}\"");
                result.Message = $"It is impossible to add next countries: {string.Join(", ", countryNames)} because they already exist";
                return result;
            }

            await _countries.CreateAsync(items);
            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            _log.Error(ex);
            return result;
        }
    }
    public async Task<OperationResult> DeleteAsync(IEnumerable<int> itemIds)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            var citiesList = await _cities.GetItemsListAsync();
            if (citiesList.Any(x => itemIds.Contains(x.CountryId)))
            {
                var countryIds = citiesList
                                    .Where(x => itemIds.Contains(x.CountryId))
                                    .Select(x => x.CountryId);
                var countryNames = (await _countries.GetItemsListAsync())
                                                .Where(x => countryIds.Contains(x.Id))
                                                .Select(x => $"\"{x.Name}\"");

                result.Message = $"It is impossible to delete next countries: {string.Join(", ", countryNames)} because there are cities associated with it";
                return result;
            }

            await _countries.DeleteAsync(itemIds);
            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            _log.Error(ex);
            return result;
        }
    }

    public async Task<Country?> GetAsync(int id)
    {
        return await _countries.GetAsync(id);
    }

    public async Task<OperationResult> UpdateAsync(IEnumerable<Country> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            var originalItems = await _countries.GetItemsListAsync();
            foreach (var item in items)
            {
                var existedItem = originalItems.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase) && x.Id != item.Id);
                if (existedItem != null)
                {
                    result.Message = $"It is impossible to update next country: {existedItem.Name} because a country with that name already exists";
                    return result;
                }

                var newItem = items.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase) && x.Id != item.Id);
                if (newItem != null)
                {
                    result.Message = $"Same name for country: {newItem.Name}";
                    return result;
                }
            }

            await _countries.UpdateAsync(items);
            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            _log.Error(ex);
            return result;
        }
    }

    public async Task<bool> AnyAsync()
    {
        return await _countries.AnyAsync();
    }
}
