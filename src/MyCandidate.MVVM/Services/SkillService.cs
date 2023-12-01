using System.Collections.Generic;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class SkillService : IDictionaryService<Skill>
{
    private readonly IDataAccess<Skill> _skills;
    public SkillService(IDataAccess<Skill> skills)
    {
        _skills = skills;
    }    
    
    public IEnumerable<Skill> ItemsList => _skills.ItemsList;

    public bool Create(IEnumerable<Skill> items, out string message)
    {
        message = string.Empty;
        _skills.Create(items);
        return true;
    }

    public bool Delete(IEnumerable<int> itemIds, out string message)
    {
        message = string.Empty;
        _skills.Delete(itemIds);
        return true;
    }

    public bool Update(IEnumerable<Skill> items, out string message)
    {
        message = string.Empty;
        _skills.Update(items);
        return true;
    }
}
