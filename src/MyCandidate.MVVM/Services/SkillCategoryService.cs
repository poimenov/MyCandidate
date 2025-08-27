using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<IEnumerable<SkillCategory>> GetItemsListAsync()
    {
        return await _categories.GetItemsListAsync();
    }

    public async Task<OperationResult> CreateAsync(IEnumerable<SkillCategory> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            if (items.Count() > 0)
            {
                var itemList = await _categories.GetItemsListAsync();
                if (itemList.Any(x => items.Select(y => y.Name).Contains(x.Name, StringComparer.InvariantCultureIgnoreCase)))
                {
                    var countryNames = items
                        .Where(x => itemList.Select(y => y.Name)
                        .Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
                        .Select(x => $"\"{x.Name}\"");
                    result.Message = $"It is impossible to add next categories: {string.Join(", ", countryNames)} because they already exist";
                    return result;
                }

                await _categories.CreateAsync(items);
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
                var skillsList = await _skills.GetItemsListAsync();
                if (skillsList.Any(x => itemIds.Contains(x.SkillCategoryId)))
                {
                    var categoryIds = skillsList
                                        .Where(x => itemIds.Contains(x.SkillCategoryId))
                                        .Select(x => x.SkillCategoryId);
                    var categoryNames = (await _categories.GetItemsListAsync())
                                                    .Where(x => categoryIds.Contains(x.Id))
                                                    .Select(x => $"\"{x.Name}\"");

                    result.Message = $"It is impossible to delete next categories: {string.Join(", ", categoryNames)} because there are skills associated with it";
                    return result;
                }

                await _categories.DeleteAsync(itemIds);
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

    public async Task<SkillCategory?> GetAsync(int id)
    {
        return await _categories.GetAsync(id);
    }

    public async Task<OperationResult> UpdateAsync(IEnumerable<SkillCategory> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            if (items.Count() > 0)
            {
                await _categories.UpdateAsync(items);
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
        return await _categories.AnyAsync();
    }
}
