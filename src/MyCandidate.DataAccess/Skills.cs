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

    public IEnumerable<Skill> ItemsList
    {
        get
        {
            using (var db = _databaseFactory.CreateDbContext())
            {
                return db.Skills.Include(x => x.SkillCategory).ToList();
            }
        }
    }

    public void Create(IEnumerable<Skill> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = _databaseFactory.CreateDbContext())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (!db.Skills.Any(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                    {
                        item.SkillCategory = null;
                        db.Skills.Add(item);
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
                    if (db.Skills.Any(x => x.Id == id))
                    {
                        var item = db.Skills.First(x => x.Id == id);
                        db.Skills.Remove(item);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public Skill? Get(int itemId)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.Skills.FirstOrDefault(x => x.Id == itemId);
        }
    }

    public void Update(IEnumerable<Skill> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = _databaseFactory.CreateDbContext())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (db.Skills.Any(x => x.Id == item.Id))
                    {
                        var entity = db.Skills.First(x => x.Id == item.Id);
                        entity.SkillCategoryId = item.SkillCategoryId;
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
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.Skills.Any(x => x.Enabled == true);
        }
    }
}
