using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class SkillCategoryService : IDictionaryService<SkillCategory>
{
    private readonly IDataAccess<SkillCategory> _categories;
    private readonly IDataAccess<Skill> _skills;
    private readonly ILog _log;    
    public SkillCategoryService(IDataAccess<SkillCategory> categories, IDataAccess<Skill> skills, ILog log)
    {
        _categories = categories;
        _skills = skills;
        _log = log;
    }
    
    public IEnumerable<SkillCategory> ItemsList => _categories.ItemsList;

    public bool Create(IEnumerable<SkillCategory> items, out string message)
    {
        message = string.Empty;
        var itemList = ItemsList;
        if (itemList.Any(x => items.Select(y => y.Name).Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)))
        {
            var countryNames = items
                .Where(x => itemList.Select(y => y.Name)
                .Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
                .Select(x => $"\"{x.Name}\"");
            message = $"It is impossible to add next categories: {string.Join(", ", countryNames)} because they already exist";
            return false;
        }

        try
        {
            _categories.Create(items);
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
        var skillsList = _skills.ItemsList;
        if (skillsList.Any(x => itemIds.Contains(x.SkillCategoryId)))
        {
            var countryIds = skillsList
                                .Where(x => itemIds.Contains(x.SkillCategoryId))
                                .Select(x => x.SkillCategoryId);
            var categoryNames = _categories.ItemsList
                                            .Where(x => countryIds.Contains(x.Id))
                                            .Select(x => $"\"{x.Name}\"");

            message = $"It is impossible to delete next categories: {string.Join(", ", categoryNames)} because there are skills associated with it";
            return false;
        }

        try
        {
            _categories.Delete(itemIds);
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }

    public bool Update(IEnumerable<SkillCategory> items, out string message)
    {
        message = string.Empty;

        var originalItems = _categories.ItemsList;
        foreach(var item in items)
        {
            var existedItem = originalItems.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase) && x.Id != item.Id);            
            if(existedItem != null)
            {
                message = $"It is impossible to update next category: {existedItem.Name} because a category with that name already exists";
                return false;
            }

            var newItem = items.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase) && x.Id != item.Id); 
            if(newItem != null)
            {
                message = $"Same name for category: {newItem.Name}";
                return false;
            }                       
        }

        try
        {
            _categories.Update(items);
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }
}
