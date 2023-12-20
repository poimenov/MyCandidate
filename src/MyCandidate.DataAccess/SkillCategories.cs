using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class SkillCategories : IDataAccess<SkillCategory>
{
    public SkillCategories()
    {
        //
    }

    public IEnumerable<SkillCategory> ItemsList
    {
        get
        {
            using (var db = new Database())
            {
                return db.SkillCategories.ToList();
            }
        }
    }

    public void Create(IEnumerable<SkillCategory> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (!db.SkillCategories.Any(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        db.SkillCategories.Add(item);
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
                    if (db.SkillCategories.Any(x => x.Id == id))
                    {
                        var item = db.SkillCategories.First(x => x.Id == id);
                        db.SkillCategories.Remove(item);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public SkillCategory? Get(int itemId)
    {
        using (var db = new Database())
        {
            return db.SkillCategories.FirstOrDefault(x => x.Id == itemId);
        }
    }

    public void Update(IEnumerable<SkillCategory> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (db.SkillCategories.Any(x => x.Id == item.Id))
                    {
                        var entity = db.SkillCategories.First(x => x.Id == item.Id);
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
