using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Skills : IDataAccess<Skill>
{
    private readonly IDatabaseFactory _databaseFactory;
    public Skills(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<IEnumerable<Skill>> GetItemsListAsync()
    {
        await using var db = _databaseFactory.CreateDbContext();
        return await db.Skills.Include(x => x.SkillCategory).ToListAsync();
    }

    public async Task CreateAsync(IEnumerable<Skill> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using var db = _databaseFactory.CreateDbContext();
        await using var transaction = await db.Database.BeginTransactionAsync();
        foreach (var item in items)
        {
            if (!await db.Skills.AnyAsync(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
            {
                item.SkillCategory = null;
                await db.Skills.AddAsync(item);
            }
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task DeleteAsync(IEnumerable<int> itemIds)
    {
        if (null == itemIds || itemIds.Count() == 0)
            return;
        await using var db = _databaseFactory.CreateDbContext();
        await using var transaction = await db.Database.BeginTransactionAsync();
        foreach (var id in itemIds)
        {
            if (await db.Skills.AnyAsync(x => x.Id == id))
            {
                var item = await db.Skills.FirstAsync(x => x.Id == id);
                db.Skills.Remove(item);
            }
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<Skill?> GetAsync(int itemId)
    {
        await using var db = _databaseFactory.CreateDbContext();
        return await db.Skills.Include(x => x.SkillCategory).FirstOrDefaultAsync(x => x.Id == itemId);
    }

    public async Task UpdateAsync(IEnumerable<Skill> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using var db = _databaseFactory.CreateDbContext();
        await using var transaction = await db.Database.BeginTransactionAsync();
        foreach (var item in items)
        {
            if (await db.Skills.AnyAsync(x => x.Id == item.Id)
                && !await db.Skills.Where(x => x.Id != item.Id
                    && x.SkillCategoryId == item.SkillCategoryId
                    && x.Name.ToLower() == item.Name.ToLower()).AnyAsync())
            {
                var entity = await db.Skills.FirstAsync(x => x.Id == item.Id);
                entity.SkillCategoryId = item.SkillCategoryId;
                entity.Name = item.Name;
                entity.Enabled = item.Enabled;
            }
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<bool> AnyAsync()
    {
        await using var db = _databaseFactory.CreateDbContext();
        return await db.Skills.AnyAsync(x => x.Enabled == true);
    }
}
