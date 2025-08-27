using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<IEnumerable<City>> GetItemsListAsync()
    {
        return await _cities.GetItemsListAsync();
    }

    public async Task<OperationResult> CreateAsync(IEnumerable<City> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            if (items.Count() > 0)
            {
                var duplicatesCheck = await CheckDuplicatesAsync(items);
                if (duplicatesCheck.Success)
                {
                    result.Message = $"It is impossible to add next cities: {string.Join(", ", duplicatesCheck.Messages)} because they already exist";
                    return result;
                }

                await _cities.CreateAsync(items);
            }

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
            if (itemIds.Count() > 0)
            {
                await _cities.DeleteAsync(itemIds);
            }
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

    public async Task<City?> GetAsync(int id)
    {
        return await _cities.GetAsync(id);
    }

    public async Task<OperationResult> UpdateAsync(IEnumerable<City> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            if (items.Count() > 0)
            {
                await _cities.UpdateAsync(items);
            }

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
        return await _cities.AnyAsync();
    }

    private async Task<OperationResults> CheckDuplicatesAsync(IEnumerable<City> items)
    {
        var result = new OperationResults { Success = false, Messages = Enumerable.Empty<string>() };
        var existedItems = (await _cities.GetItemsListAsync()).Select(x => new KeyValuePair<int, string>(x.CountryId, x.Name.Trim().ToLower()));
        var newItems = items.Select(x => new KeyValuePair<int, string>(x.CountryId, x.Name.Trim().ToLower()));
        var allItems = new List<KeyValuePair<int, string>>();
        allItems.AddRange(existedItems);
        allItems.AddRange(newItems);
        var duplicates = allItems.Select(x => new { CountryId = x.Key, Name = x.Value })
            .GroupBy(x => new { x.CountryId, x.Name }).Where(x => x.Count() > 1);
        if (duplicates.Any())
        {
            result.Success = true;
            result.Messages = duplicates.Select(x => x.Key.Name);
        }

        return result;
    }
}
