using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Companies : IDataAccess<Company>
{
    public Companies()
    {
        //
    }

    public IEnumerable<Company> ItemsList
    {
        get
        {
            using (var db = new Database())
            {
                return db.Companies.ToList();
            }
        }
    }

    public void Create(IEnumerable<Company> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (!db.Companies.Any(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        db.Companies.Add(item);
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
                    if (db.Companies.Any(x => x.Id == id))
                    {
                        var item = db.Companies.First(x => x.Id == id);
                        db.Companies.Remove(item);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public Company? Get(int itemId)
    {
        using (var db = new Database())
        {
            return db.Companies.FirstOrDefault(x => x.Id == itemId);
        }
    }

    public void Update(IEnumerable<Company> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (db.Companies.Any(x => x.Id == item.Id))
                    {
                        var entity = db.Companies.First(x => x.Id == item.Id);
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
            return db.Companies.Any(x => x.Enabled == true);
        }
    }
}
