using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public IEnumerable<Office> ItemsList => _officies.ItemsList;

    public bool Create(IEnumerable<Office> items, out string message)
    {
        message = string.Empty;
        IEnumerable<string> names;
        if (CheckDuplicates(items, out names))
        {
            message = $"It is impossible to add next officies: {string.Join(", ", names)} because they already exist";
            return false;
        }

        try
        {
            _officies.Create(items);
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
        _officies.Delete(itemIds);
        return true;
    }

    public Office? Get(int id)
    {
        return _officies.Get(id);
    }

    public bool Update(IEnumerable<Office> items, out string message)
    {
        message = string.Empty;
        try
        {
            _officies.Update(items);
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
        return _officies.Any();
    }

    private bool CheckDuplicates(IEnumerable<Office> items, out IEnumerable<string> names)
    {
        names = Enumerable.Empty<string>();
        var existedItems = ItemsList.Select(x => new KeyValuePair<int, string>(x.CompanyId, x.Name.Trim().ToLower()));
        var newItems = items.Select(x => new KeyValuePair<int, string>(x.CompanyId, x.Name.Trim().ToLower()));
        var allItems = new List<KeyValuePair<int, string>>();
        allItems.AddRange(existedItems);
        allItems.AddRange(newItems);
        var duplicates = allItems.Select(x => new { CompanyId = x.Key, Name = x.Value })
            .GroupBy(x => new { x.CompanyId, x.Name }).Where(x => x.Count() > 1);
        if(duplicates.Any())
        {
            names = duplicates.Select(x => x.Key.Name);
            return true;
        }

        return false;
    }    
}
