using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Officies : IDataAccess<Office>
{
    public Officies()
    {
        //
    }

    public IEnumerable<Office> ItemsList
    {
        get
        {
            using (var db = new Database())
            {
                return db.Offices.Include(x => x.Company).ToList();
            }
        }        
    }

    public void Create(IEnumerable<Office> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {

            foreach (var item in items)
            {
                if (!db.Offices.Any(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()))
                {
                    item.Company = null;
                    db.Offices.Add(item);
                }
            }
            db.SaveChanges();
        }
    }

    public void Delete(IEnumerable<int> itemIds)
    {
        if (null == itemIds || itemIds.Count() == 0)
            return;
        using (var db = new Database())
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
        }
    }

    public Office? Get(int itemId)
    {
        using (var db = new Database())
        {
            return db.Offices.Include(x => x.Company).FirstOrDefault(x => x.Id == itemId);
        }
    }

    public void Update(IEnumerable<Office> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
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
                    entityLocation.Address = item.Location.Address;
                    entityLocation.CityId = item.Location.CityId;
                }
            }
            db.SaveChanges();
        }
    }
}
