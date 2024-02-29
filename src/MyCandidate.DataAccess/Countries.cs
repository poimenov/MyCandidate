using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
namespace MyCandidate.DataAccess;

public class Countries : IDataAccess<Country>
{
    public Countries()
    {
    }

    public IEnumerable<Country> ItemsList
    {
        get
        {
            using (var db = new Database())
            {
                return db.Countries.ToList();
            }
        }
    }

    public void Create(IEnumerable<Country> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (!db.Countries.Any(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        db.Countries.Add(item);
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
                    if (db.Countries.Any(x => x.Id == id))
                    {
                        var item = db.Countries.First(x => x.Id == id);
                        db.Countries.Remove(item);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public Country? Get(int itemId)
    {
        using (var db = new Database())
        {
            return db.Countries.FirstOrDefault(x => x.Id == itemId);
        }
    }

    public void Update(IEnumerable<Country> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (db.Countries.Any(x => x.Id == item.Id))
                    {
                        var entity = db.Countries.First(x => x.Id == item.Id);
                        entity.Name = item.Name;
                        entity.Enabled = item.Enabled;
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public bool Any()
    {
        using (var db = new Database())
        {
            return db.Countries.Any(x => x.Enabled == true);
        }
    }
}
