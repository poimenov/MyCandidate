using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Cities : IDataAccess<City>
{
    private readonly IDatabaseFactory _databaseFactory;

    public Cities(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<IEnumerable<City>> GetItemsListAsync()
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.Cities.Include(x => x.Country).ToListAsync();
        }
    }

    public async Task CreateAsync(IEnumerable<City> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var item in items)
                {
                    if (!await db.Cities.AnyAsync(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        item.Country = null;
                        await db.Cities.AddAsync(item);
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
                    if (await db.Cities.AnyAsync(x => x.Id == id))
                    {
                        var item = await db.Cities.FirstAsync(x => x.Id == id);
                        db.Cities.Remove(item);
                    }
                }
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
    }

    public async Task<City?> GetAsync(int itemId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.Cities.Include(x => x.Country).FirstOrDefaultAsync(x => x.Id == itemId);
        }
    }

    public async Task UpdateAsync(IEnumerable<City> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var item in items)
                {
                    if (await db.Cities.AnyAsync(x => x.Id == item.Id))
                    {
                        var entity = await db.Cities.FirstAsync(x => x.Id == item.Id);
                        entity.CountryId = item.CountryId;
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
            return await db.Cities.AnyAsync(x => x.Enabled == true);
        }
    }
}
