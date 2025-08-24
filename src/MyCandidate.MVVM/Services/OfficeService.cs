using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class OfficeService : IDictionaryService<Office>
{
    private readonly IDataAccess<Office> _officies;
    private readonly ILog _log;
    public OfficeService(IDataAccess<Office> officies, ILog log)
    {
        _officies = officies;
        _log = log;
    }

    public async Task<IEnumerable<Office>> GetItemsListAsync()
    {
        return await _officies.GetItemsListAsync();
    }

    public async Task<OperationResult> CreateAsync(IEnumerable<Office> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            var duplicatesCheck = await CheckDuplicatesAsync(items);
            if (duplicatesCheck.Success)
            {
                result.Message = $"It is impossible to add next officies: {string.Join(", ", duplicatesCheck.Messages)} because they already exist";
                return result;
            }

            await _officies.CreateAsync(items);
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
            await _officies.DeleteAsync(itemIds);
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

    public async Task<Office?> GetAsync(int id)
    {
        return await _officies.GetAsync(id);
    }

    public async Task<OperationResult> UpdateAsync(IEnumerable<Office> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            var duplicatesCheck = await CheckDuplicatesAsync(items);
            if (duplicatesCheck.Success)
            {
                result.Message = $"It is impossible to update next officies: {string.Join(", ", duplicatesCheck.Messages)} because they already exist";
                return result;
            }

            await _officies.UpdateAsync(items);
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
        return await _officies.AnyAsync();
    }

    private async Task<OperationResults> CheckDuplicatesAsync(IEnumerable<Office> items)
    {
        var result = new OperationResults { Success = false, Messages = Enumerable.Empty<string>() };
        var existedItems = (await _officies.GetItemsListAsync()).Select(x => new KeyValuePair<int, string>(x.CompanyId, x.Name.Trim().ToLower()));
        var newItems = items.Select(x => new KeyValuePair<int, string>(x.CompanyId, x.Name.Trim().ToLower()));
        var allItems = new List<KeyValuePair<int, string>>();
        allItems.AddRange(existedItems);
        allItems.AddRange(newItems);
        var duplicates = allItems.Select(x => new { CompanyId = x.Key, Name = x.Value })
            .GroupBy(x => new { x.CompanyId, x.Name }).Where(x => x.Count() > 1);
        if (duplicates.Any())
        {
            result.Success = true;
            result.Messages = duplicates.Select(x => x.Key.Name);
        }

        return result;
    }
}
