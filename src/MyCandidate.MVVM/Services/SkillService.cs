using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<IEnumerable<Skill>> GetItemsListAsync()
    {
        return await _skills.GetItemsListAsync();
    }

    public async Task<OperationResult> CreateAsync(IEnumerable<Skill> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            var duplicatesCheck = await CheckDuplicatesAsync(items);
            if (duplicatesCheck.Success)
            {
                result.Message = $"It is impossible to add next skills: {string.Join(", ", duplicatesCheck.Messages)} because they already exist";
                return result;
            }

            await _skills.CreateAsync(items);
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
            await _skills.DeleteAsync(itemIds);
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

    public async Task<Skill?> GetAsync(int id)
    {
        return await _skills.GetAsync(id);
    }

    public async Task<OperationResult> UpdateAsync(IEnumerable<Skill> items)
    {
        var result = new OperationResult { Success = false, Message = string.Empty };
        try
        {
            var duplicatesCheck = await CheckDuplicatesAsync(items);
            if (duplicatesCheck.Success)
            {
                result.Message = $"It is impossible to add next skills: {string.Join(", ", duplicatesCheck.Messages)} because they already exist";
                return result;
            }

            await _skills.UpdateAsync(items);
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
        return await _skills.AnyAsync();
    }

    private async Task<OperationResults> CheckDuplicatesAsync(IEnumerable<Skill> items)
    {
        var result = new OperationResults { Success = false, Messages = Enumerable.Empty<string>() };
        var itemList = await _skills.GetItemsListAsync();
        var newItems = items.Select(x => new KeyValuePair<int, string>(x.SkillCategoryId, x.Name.Trim().ToLower()));
        var existedItems = itemList.Select(x => new KeyValuePair<int, string>(x.SkillCategoryId, x.Name.Trim().ToLower()));
        var allItems = new List<KeyValuePair<int, string>>();
        allItems.AddRange(existedItems);
        allItems.AddRange(newItems);
        var duplicates = allItems.Select(x => new { SkillCategoryId = x.Key, Name = x.Value })
            .GroupBy(x => new { x.SkillCategoryId, x.Name }).Where(x => x.Count() > 1);
        if (duplicates.Any())
        {
            result.Success = true;
            result.Messages = duplicates.Select(x => x.Key.Name);
        }

        return result;
    }
}
