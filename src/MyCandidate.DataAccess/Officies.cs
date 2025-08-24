using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Officies : IDataAccess<Office>
{
    private readonly IDatabaseFactory _databaseFactory;
    public Officies(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<IEnumerable<Office>> GetItemsListAsync()
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.Offices.Include(x => x.Company)
                .Include(x => x.Location!)
                .ThenInclude(x => x.City!)
                .ThenInclude(x => x.Country)
                .ToListAsync();
        }
    }

    public async Task CreateAsync(IEnumerable<Office> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var item in items)
                {
                    if (!await db.Offices.AnyAsync(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower() && x.CompanyId == item.CompanyId))
                    {
                        item.Company = null;
                        if (item.Location != null)
                        {
                            item.Location.City = null;
                            item.Location.Address = item.Location.Address ?? string.Empty;
                        }

                        await db.Offices.AddAsync(item);
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
                    if (await db.Offices.AnyAsync(x => x.Id == id))
                    {
                        var item = await db.Offices.FirstAsync(x => x.Id == id);
                        var itemLocation = await db.Locations.FirstAsync(x => x.Id == item.LocationId);
                        db.Offices.Remove(item);
                        db.Locations.Remove(itemLocation);
                    }
                }
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
    }

    public async Task<Office?> GetAsync(int itemId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.Offices.Include(x => x.Company)
                .Include(x => x.Location!)
                .ThenInclude(x => x.City!)
                .ThenInclude(x => x.Country)
                .FirstOrDefaultAsync(x => x.Id == itemId);
        }
    }

    public async Task UpdateAsync(IEnumerable<Office> items)
    {
        if (null == items || items.Count() == 0)
            return;
        await using (var db = _databaseFactory.CreateDbContext())
        {
            await using (var transaction = await db.Database.BeginTransactionAsync())
            {
                foreach (var item in items)
                {
                    if (await db.Offices.AnyAsync(x => x.Id == item.Id))
                    {
                        var entity = await db.Offices.FirstAsync(x => x.Id == item.Id);
                        entity.CompanyId = item.CompanyId;
                        entity.Name = item.Name;
                        entity.Enabled = item.Enabled;

                        if (item.Location != null)
                        {
                            var entityLocation = await db.Locations.FirstAsync(x => x.Id == entity.LocationId);
                            entityLocation.Address = item.Location.Address ?? string.Empty;
                            entityLocation.CityId = item.Location.CityId;
                        }
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
            return await db.Offices.AnyAsync(x => x.Enabled == true);
        }
    }
}
