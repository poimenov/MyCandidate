using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Candidates : ICandidates
{
    private readonly ICandidateOnVacancies _candidateOnVacancies;
    public Candidates(ICandidateOnVacancies candidateOnVacancies)
    {
        _candidateOnVacancies = candidateOnVacancies;
    }

    public bool Exist(string lastName, string firstName, DateTime birthdate)
    {
        using (var db = new Database())
        {
            return db.Candidates.Any(x => x.LastName.Trim().ToLower() == lastName.Trim().ToLower()
                                            && x.FirstName.Trim().ToLower() == firstName.Trim().ToLower()
                                            && x.BirthDate == birthdate);
        }
    }

    public void Delete(int id)
    {
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (db.Candidates.Any(x => x.Id == id))
                {
                    try
                    {
                        if (db.CandidateResources.Any(x => x.CandidateId == id))
                        {
                            db.CandidateResources.RemoveRange(db.CandidateResources.Where(x => x.CandidateId == id));
                        }

                        if (db.CandidateSkills.Any(x => x.CandidateId == id))
                        {
                            db.CandidateSkills.RemoveRange(db.CandidateSkills.Where(x => x.CandidateId == id));
                        }

                        var item = db.Candidates.First(x => x.Id == id);
                        db.Locations.Remove(db.Locations.First(x => x.Id == item.LocationId));
                        _candidateOnVacancies.DeleteByCandidateId(id);
                        db.Candidates.Remove(item);

                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (System.Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

            }
        }
    }

    public bool Create(Candidate candidate, out int id)
    {
        bool retVal = false;
        id = 0;

        if (Exist(candidate.LastName, candidate.FirstName, candidate.BirthDate))
        {
            return retVal;
        }

        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var newLocation = new Location
                    {
                        Address = candidate.Location.Address ?? string.Empty,
                        CityId = candidate.Location.CityId
                    };
                    db.Locations.Add(newLocation);
                    db.SaveChanges();

                    var newCandidate = new Candidate
                    {
                        LastName = candidate.LastName,
                        FirstName = candidate.FirstName,
                        BirthDate = candidate.BirthDate,
                        LocationId = newLocation.Id,
                        CreationDate = DateTime.Now,
                        LastModificationDate = DateTime.Now
                    };
                    db.Candidates.Add(newCandidate);
                    db.SaveChanges();
                    id = newCandidate.Id;

                    foreach (var candidateResource in candidate.CandidateResources)
                    {
                        var newCandidateResource = new CandidateResource
                        {
                            CandidateId = id,
                            Value = candidateResource.Value,
                            ResourceTypeId = candidateResource.ResourceTypeId
                        };
                        db.CandidateResources.Add(newCandidateResource);
                    }

                    foreach (var candidateSkill in candidate.CandidateSkills)
                    {
                        var newCandidateSkill = new CandidateSkill
                        {
                            CandidateId = id,
                            SeniorityId = candidateSkill.SeniorityId,
                            SkillId = candidateSkill.SkillId
                        };
                        db.CandidateSkills.Add(newCandidateSkill);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    retVal = true;
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        return retVal;
    }

    public void Update(Candidate candidate)
    {
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (db.Candidates.Any(x => x.Id == candidate.Id))
                    {
                        var entity = db.Candidates.First(x => x.Id == candidate.Id);
                        entity.LastModificationDate = DateTime.Now;
                        entity.FirstName = candidate.FirstName;
                        entity.LastName = candidate.LastName;
                        entity.BirthDate = candidate.BirthDate;
                        entity.Enabled = candidate.Enabled;

                        var location = db.Locations.First(x => x.Id == entity.LocationId);
                        location.Address = candidate.Location.Address;
                        location.CityId = candidate.Location.CityId;

                        //update existed resources
                        var idsToUpdate = candidate.CandidateResources.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
                        foreach (var idToUpdate in idsToUpdate)
                        {
                            if (db.CandidateResources.Any(x => x.Id == idToUpdate))
                            {
                                var resourceToUpdate = candidate.CandidateResources.First(x => x.Id == idToUpdate);
                                var resource = db.CandidateResources.First(x => x.Id == idToUpdate);
                                resource.Value = resourceToUpdate.Value;
                                resource.ResourceTypeId = resourceToUpdate.ResourceTypeId;
                            }
                        }
                        //delete existed resources
                        var idsToDelete = db.CandidateResources.Where(x => x.CandidateId == candidate.Id && !idsToUpdate.Contains(x.Id))
                                                                       .Select(x => x.Id).ToArray();
                        foreach (var idToDelete in idsToDelete)
                        {
                            if (db.CandidateResources.Any(x => x.Id == idToDelete))
                            {
                                var resource = db.CandidateResources.First(x => x.Id == idToDelete);
                                db.CandidateResources.Remove(resource);
                            }
                        }
                        //add new resources
                        foreach (var resourceToAdd in candidate.CandidateResources.Where(x => x.Id == 0))
                        {
                            var newResource = new CandidateResource
                            {
                                CandidateId = candidate.Id,
                                Value = resourceToAdd.Value,
                                ResourceTypeId = resourceToAdd.ResourceTypeId
                            };
                            db.CandidateResources.Add(newResource);
                        }

                        //update existed skills
                        idsToUpdate = candidate.CandidateSkills.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
                        foreach (var idToUpdate in idsToUpdate)
                        {
                            if (db.CandidateSkills.Any(x => x.Id == idToUpdate))
                            {
                                var skillToUpdate = candidate.CandidateSkills.First(x => x.Id == idToUpdate);
                                var skill = db.CandidateSkills.First(x => x.Id == idToUpdate);
                                skill.SeniorityId = skillToUpdate.SeniorityId;
                                skill.SkillId = skillToUpdate.SkillId;
                            }
                        }
                        //delete existed skills
                        idsToDelete = db.CandidateSkills.Where(x => x.CandidateId == candidate.Id && !idsToUpdate.Contains(x.Id))
                                                                       .Select(x => x.Id).ToArray();
                        foreach (var idToDelete in idsToDelete)
                        {
                            if (db.CandidateSkills.Any(x => x.Id == idToDelete))
                            {
                                var skill = db.CandidateSkills.First(x => x.Id == idToDelete);
                                db.CandidateSkills.Remove(skill);
                            }
                        }
                        //add new skills
                        foreach (var skillToAdd in candidate.CandidateSkills.Where(x => x.Id == 0))
                        {
                            var newSkill = new CandidateSkill
                            {
                                CandidateId = candidate.Id,
                                SeniorityId = skillToAdd.SeniorityId,
                                SkillId = skillToAdd.SkillId
                            };
                            db.CandidateSkills.Add(newSkill);
                        }

                        _candidateOnVacancies.Update(candidate.CandidateOnVacancies);

                        db.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public Candidate Get(int id)
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
                .First(x => x.Id == id);
        }
    }

    public IEnumerable<Candidate> Search(CandidateSearch searchParams)
    {
        using (var db = new Database())
        {
            var retVal = db.Candidates.AsQueryable();

            if (searchParams.Enabled.HasValue)
            {
                retVal = retVal.Where(x => x.Enabled == searchParams.Enabled.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.LastName))
            {
                retVal = retVal.Where(x => x.LastName.ToLower().StartsWith(searchParams.LastName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.FirstName))
            {
                retVal = retVal.Where(x => x.FirstName.ToLower().StartsWith(searchParams.FirstName.ToLower()));
            }

            if (searchParams.CityId.HasValue)
            {
                retVal = retVal.Where(x => x.Location.CityId == searchParams.CityId.Value);
            }
            else if (searchParams.CountryId.HasValue)
            {
                retVal = retVal.Where(x => x.Location.City.CountryId == searchParams.CountryId.Value);
            }

            if (searchParams.Skills.Any())
            {
                foreach (var skill in searchParams.Skills)
                {
                    retVal = retVal.Where(x => x.CandidateSkills.Any(s => s.SkillId == skill.SkillId && s.SeniorityId == skill.SeniorityId));
                }
            }

            return retVal.Include(x => x.Location)
                .ThenInclude(x => x.City)
                .ThenInclude(x => x.Country)
                .Include(x => x.CandidateOnVacancies)
                .ToList();
        }
    }
}
