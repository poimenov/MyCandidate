using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Candidates : IDataAccess<Candidate>
{
    public Candidates()
    {
        //
    }

    public IEnumerable<Candidate> ItemsList
    {
        get
        {
            using (var db = new Database())
            {
                return db.Candidates
                    .Include(x => x.Location)
                    .Include(x => x.CandidateSkills)
                    .Include(x => x.CandidateOnVacancies)
                    .ToList();
            }
        }
    }

    public void Create(IEnumerable<Candidate> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (!db.Candidates.Any(x => Equals(x, item)))
                    {
                        item.CreationDate = DateTime.Now;
                        item.LastModificationDate = DateTime.Now;
                        item.Location.City = null;
                        item.CandidateResources.ForEach(x => x.ResourceType = null);
                        item.CandidateSkills.ForEach(x =>
                            {
                                x.Skill = null;
                                x.Seniority = null;
                            });
                        db.Candidates.Add(item);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    private bool Equals(Candidate x, Candidate y) =>
            x.FirstName.Equals(y.FirstName, StringComparison.InvariantCultureIgnoreCase)
            && x.LastName.Equals(y.LastName, StringComparison.InvariantCultureIgnoreCase)
            && x.BirthDate.Equals(y.BirthDate);

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
                    if (db.Candidates.Any(x => x.Id == id))
                    {
                        var item = db.Candidates.First(x => x.Id == id);
                        db.Candidates.Remove(item);
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }

    public Candidate? Get(int itemId)
    {
        using (var db = new Database())
        {
            return db.Candidates
                .Include(x => x.Location)
                .ThenInclude(x => x.City)
                .ThenInclude(x => x.Country)
                .Include(x => x.CandidateSkills)
                .ThenInclude(x => x.Seniority)
                .Include(x => x.CandidateSkills)
                .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.SkillCategory)
                .Include(x => x.CandidateResources)
                .ThenInclude(x => x.ResourceType)
                .Include(x => x.CandidateOnVacancies)
                .ThenInclude(x => x.Vacancy)
                .FirstOrDefault(x => x.Id == itemId);
        }
    }

    public void Update(IEnumerable<Candidate> items)
    {
        if (null == items || items.Count() == 0)
            return;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                foreach (var item in items)
                {
                    if (db.Candidates.Any(x => x.Id == item.Id))
                    {
                        // var resources = db.CandidateResources.Where(x => x.CandidateId == item.Id);
                        // db.CandidateResources.RemoveRange(resources);
                        // db.SaveChanges();

                        // var skills = db.CandidateSkills.Where(x => x.CandidateId == item.Id);
                        // db.CandidateSkills.RemoveRange(skills);
                        // db.SaveChanges();

                        var entity = db.Candidates.First(x => x.Id == item.Id);
                        entity.LastModificationDate = DateTime.Now;
                        entity.CandidateResources = item.CandidateResources;
                        entity.CandidateResources.ForEach(x => x.ResourceType = null);
                        entity.CandidateSkills = item.CandidateSkills;
                        entity.CandidateSkills.ForEach(x =>
                            {
                                x.Skill = null;
                                x.Seniority = null;
                            });
                        entity.Location.Address = item.Location.Address;
                        entity.Location.CityId = item.Location.CityId;
                        entity.FirstName = item.FirstName;
                        entity.LastName = item.LastName;
                        entity.Enabled = item.Enabled;
                    }
                }
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }
}
