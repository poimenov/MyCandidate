using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class CompanyService : IDictionaryService<Company>
{
    private readonly IDataAccess<Company> _companies;
    private readonly IDataAccess<Office> _officies;
    private readonly ILog _log;
    public CompanyService(IDataAccess<Company> companies, IDataAccess<Office> officies, ILog log)
    {
        _companies = companies;
        _officies = officies;
        _log = log;
    }

    public async Task<IEnumerable<Company>> GetItemsListAsync()
    {
        return await _companies.GetItemsListAsync();
    }

    public async Task<OperationResult> CreateAsync(IEnumerable<Company> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            if (items.Count() > 0)
            {
                var itemList = await _companies.GetItemsListAsync();
                if (itemList.Any(x => items.Select(y => y.Name).Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)))
                {
                    var countryNames = items
                        .Where(x => itemList.Select(y => y.Name)
                        .Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
                        .Select(x => $"\"{x.Name}\"");
                    result.Message = $"It is impossible to add next companies: {string.Join(", ", countryNames)} because they already exist";
                    return result;
                }

                await _companies.CreateAsync(items);
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
                var officiesList = await _officies.GetItemsListAsync();
                if (officiesList.Any(x => itemIds.Contains(x.CompanyId)))
                {
                    var companyIds = officiesList
                                        .Where(x => itemIds.Contains(x.CompanyId))
                                        .Select(x => x.CompanyId);
                    var companyNames = (await _companies.GetItemsListAsync())
                                                    .Where(x => companyIds.Contains(x.Id))
                                                    .Select(x => $"\"{x.Name}\"");
                    result.Message = $"It is impossible to delete next companies: {string.Join(", ", companyNames)} because there are officies associated with it";
                    return result;
                }

                await _companies.DeleteAsync(itemIds);
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
    public async Task<Company?> GetAsync(int id)
    {
        return await _companies.GetAsync(id);
    }

    public async Task<OperationResult> UpdateAsync(IEnumerable<Company> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            if (items.Count() > 0)
            {
                await _companies.UpdateAsync(items);
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
        return await _companies.AnyAsync();
    }
}
