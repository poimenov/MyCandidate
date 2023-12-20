using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Cities : IDataAccess<City>
{
    public Cities()
    {
        //
    }

    public IEnumerable<City> ItemsList
    {
        get
        {
            using (var db = new Database())
            {
                return db.Cities.Include(x => x.Country).ToList();
            }
        }
    }

    public void Create(IEnumerable<City> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (!db.Cities.Any(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        item.Country = null;
                        db.Cities.Add(item);
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
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var id in itemIds)
                {
                    if (db.Cities.Any(x => x.Id == id))
                    {
                        var item = db.Cities.First(x => x.Id == id);
                        db.Cities.Remove(item);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public City? Get(int itemId)
    {
        using (var db = new Database())
        {
            return db.Cities.Include(x => x.Country).FirstOrDefault(x => x.Id == itemId);
        }
    }

    public void Update(IEnumerable<City> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (db.Cities.Any(x => x.Id == item.Id))
                    {
                        var entity = db.Cities.First(x => x.Id == item.Id);
                        entity.CountryId = item.CountryId;
                        entity.Name = item.Name;
                        entity.Enabled = item.Enabled;
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }
}
