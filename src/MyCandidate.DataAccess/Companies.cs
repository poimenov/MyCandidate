using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Companies : IDataAccess<Company>
{
    private readonly IDatabaseFactory _databaseFactory;
    public Companies(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<IEnumerable<Company>> GetItemsListAsync()
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.Companies.ToListAsync();
        }
    }

    public async Task CreateAsync(IEnumerable<Company> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var item in items)
                {
                    if (!await db.Companies.AnyAsync(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        await db.Companies.AddAsync(item);
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
                    if (await db.Companies.AnyAsync(x => x.Id == id))
                    {
                        var item = await db.Companies.FirstAsync(x => x.Id == id);
                        db.Companies.Remove(item);
                    }
                }
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
    }

    public async Task<Company?> GetAsync(int itemId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.Companies.FirstOrDefaultAsync(x => x.Id == itemId);
        }
    }

    public async Task UpdateAsync(IEnumerable<Company> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var item in items)
                {
                    if (await db.Companies.AnyAsync(x => x.Id == item.Id)
                        && !await db.Companies.Where(x => x.Id != item.Id
                            && x.Name.ToLower() == item.Name.ToLower()).AnyAsync())
                    {
                        var entity = await db.Companies.FirstAsync(x => x.Id == item.Id);
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
            return await db.Companies.AnyAsync(x => x.Enabled == true);
        }
    }
}
