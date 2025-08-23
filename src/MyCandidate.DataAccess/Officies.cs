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

    public IEnumerable<Office> ItemsList
    {
        get
        {
            using (var db = _databaseFactory.CreateDbContext())
            {
                return db.Offices.Include(x => x.Company)
                    .Include(x => x.Location!)
                    .ThenInclude(x => x.City!)
                    .ThenInclude(x => x.Country)
                    .ToList();
            }
        }
    }

    public void Create(IEnumerable<Office> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = _databaseFactory.CreateDbContext())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (!db.Offices.Any(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower() && x.CompanyId == item.CompanyId))
                    {
                        item.Company = null;
                        if (item.Location != null)
                        {
                            item.Location.City = null;
                            item.Location.Address = item.Location.Address ?? string.Empty;
                        }

                        db.Offices.Add(item);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public void Delete(IEnumerable<int> itemIds)
    {
        if (null == itemIds || itemIds.Count() == 0)
            return;
        using (var db = _databaseFactory.CreateDbContext())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var id in itemIds)
                {
                    if (db.Offices.Any(x => x.Id == id))
                    {
                        var item = db.Offices.First(x => x.Id == id);
                        var itemLocation = db.Locations.First(x => x.Id == item.LocationId);
                        db.Offices.Remove(item);
                        db.Locations.Remove(itemLocation);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public Office? Get(int itemId)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.Offices.Include(x => x.Company)
                .Include(x => x.Location!)
                .ThenInclude(x => x.City!)
                .ThenInclude(x => x.Country)
                .FirstOrDefault(x => x.Id == itemId);
        }
    }

    public void Update(IEnumerable<Office> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = _databaseFactory.CreateDbContext())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (db.Offices.Any(x => x.Id == item.Id))
                    {
                        var entity = db.Offices.First(x => x.Id == item.Id);
                        entity.CompanyId = item.CompanyId;
                        entity.Name = item.Name;
                        entity.Enabled = item.Enabled;

                        var entityLocation = db.Locations.First(x => x.Id == entity.LocationId);
                        entityLocation.Address = item.Location!.Address;
                        entityLocation.CityId = item.Location!.CityId;
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public bool Any()
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.Offices.Any(x => x.Enabled == true);
        }
    }
}
