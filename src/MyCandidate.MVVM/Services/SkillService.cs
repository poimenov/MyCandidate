using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class SkillService : IDictionaryService<Skill>
{
    private readonly IDataAccess<Skill> _skills;
    private readonly ILog _log;
    public SkillService(IDataAccess<Skill> skills, ILog log)
    {
        _skills = skills;
        _log = log;
    }    
    
    public IEnumerable<Skill> ItemsList => _skills.ItemsList;

    public bool Create(IEnumerable<Skill> items, out string message)
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
            _skills.Create(items);
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
        _skills.Delete(itemIds);
        return true;
    }

    public Skill Get(int id)
    {
        return _skills.Get(id);
    }

    public bool Update(IEnumerable<Skill> items, out string message)
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
            _skills.Update(items);
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }

    private bool CheckDuplicates(IEnumerable<Skill> items, out IEnumerable<string> names)
    {
        names = Enumerable.Empty<string>();
        var existedItems = ItemsList.Select(x => new KeyValuePair<int, string>(x.SkillCategoryId, x.Name.Trim().ToLower()));
        var newItems = items.Select(x => new KeyValuePair<int, string>(x.SkillCategoryId, x.Name.Trim().ToLower()));
        var allItems = new List<KeyValuePair<int, string>>();
        allItems.AddRange(existedItems);
        allItems.AddRange(newItems);
        var duplicates = allItems.Select(x => new { SkillCategoryId = x.Key, Name = x.Value })
            .GroupBy(x => new { x.SkillCategoryId, x.Name }).Where(x => x.Count() > 1);
        if(duplicates.Any())
        {
            names = duplicates.Select(x => x.Key.Name);
            return true;
        }

        return false;
    }     
}
