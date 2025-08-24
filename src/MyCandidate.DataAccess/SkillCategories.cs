using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class SkillCategories : IDataAccess<SkillCategory>
{
    private readonly IDatabaseFactory _databaseFactory;
    public SkillCategories(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<IEnumerable<SkillCategory>> GetItemsListAsync()
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.SkillCategories.ToListAsync();
        }
    }

    public async Task CreateAsync(IEnumerable<SkillCategory> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var item in items)
                {
                    if (!await db.SkillCategories.AnyAsync(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        await db.SkillCategories.AddAsync(item);
                    }
                }
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
    }

    public async Task DeleteAsync(IEnumerable<int> itemIds)
    {
        if (null == itemIds || itemIds.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var id in itemIds)
                {
                    if (await db.SkillCategories.AnyAsync(x => x.Id == id))
                    {
                        var item = await db.SkillCategories.FirstAsync(x => x.Id == id);
                        db.SkillCategories.Remove(item);
                    }
                }
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
    }

    public async Task<SkillCategory?> GetAsync(int itemId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.SkillCategories.FirstOrDefaultAsync(x => x.Id == itemId);
        }
    }

    public async Task UpdateAsync(IEnumerable<SkillCategory> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var item in items)
                {
                    if (await db.SkillCategories.AnyAsync(x => x.Id == item.Id))
                    {
                        var entity = await db.SkillCategories.FirstAsync(x => x.Id == item.Id);
                        entity.Name = item.Name;
                        entity.Enabled = item.Enabled;
                    }
                }
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
    }

    public async Task<bool> AnyAsync()
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.SkillCategories.AnyAsync(x => x.Enabled == true);
        }
    }
}
