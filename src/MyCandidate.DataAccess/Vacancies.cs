using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Vacancies : IVacancies
{
    public Vacancies()
    {
        //
    }

    public bool Create(Vacancy vacancy, out int id)
    {
        bool retVal = false;
        id = 0;

        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var newVacancy = new Vacancy
                    {
                        Name = vacancy.Name,
                        Description = vacancy.Description,
                        Enabled = vacancy.Enabled,
                        OfficeId = vacancy.OfficeId,
                        VacancyStatusId = vacancy.VacancyStatusId,
                        CreationDate = DateTime.Now,
                        LastModificationDate = DateTime.Now
                    };
                    db.Vacancies.Add(newVacancy);
                    db.SaveChanges();
                    id = newVacancy.Id;

                    foreach (var VacancyResource in vacancy.VacancyResources)
                    {
                        var newVacancyResource = new VacancyResource
                        {
                            VacancyId = id,
                            Value = VacancyResource.Value,
                            ResourceTypeId = VacancyResource.ResourceTypeId
                        };
                        db.VacancyResources.Add(newVacancyResource);
                    }

                    foreach (var VacancySkill in vacancy.VacancySkills)
                    {
                        var newVacancySkill = new VacancySkill
                        {
                            VacancyId = id,
                            SeniorityId = VacancySkill.SeniorityId,
                            SkillId = VacancySkill.SkillId
                        };
                        db.VacancySkills.Add(newVacancySkill);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    retVal = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        return retVal;
    }

    public void Delete(int id)
    {
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (db.Vacancies.Any(x => x.Id == id))
                {
                    try
                    {
                        if (db.VacancyResources.Any(x => x.VacancyId == id))
                        {
                            db.VacancyResources.RemoveRange(db.VacancyResources.Where(x => x.VacancyId == id));
                        }

                        if (db.VacancySkills.Any(x => x.VacancyId == id))
                        {
                            db.VacancySkills.RemoveRange(db.VacancySkills.Where(x => x.VacancyId == id));
                        }

                        var item = db.Vacancies.First(x => x.Id == id);
                        db.Vacancies.Remove(item);

                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

            }
        }
    }

    public Vacancy Get(int id)
    {
        using (var db = new Database())
        {
            return db.Vacancies
                .Include(x => x.Office)
                .ThenInclude(x => x.Company)
                .Include(x => x.VacancyStatus)
                .Include(x => x.VacancyResources)
                .ThenInclude(x => x.ResourceType)
                .Include(x => x.VacancySkills)
                .ThenInclude(x => x.Seniority)
                .Include(x => x.VacancySkills)
                .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.SkillCategory) 
                .Include(x => x.CandidateOnVacancies)                   
                .First(x => x.Id == id);
        }
    }

    public IEnumerable<Vacancy> Search(VacancySearch searchParams)
    {
        using (var db = new Database())
        {
            var retVal = db.Vacancies.AsQueryable();

            if(searchParams.Enabled.HasValue)
            {
                retVal = retVal.Where(x => x.Enabled == searchParams.Enabled.Value);
            }            

            if(!string.IsNullOrWhiteSpace(searchParams.Name))
            {
                retVal = retVal.Where(x => x.Name.ToLower().Contains(searchParams.Name.ToLower()));
            }

            if(searchParams.VacancyStatusId.HasValue)
            {
                retVal = retVal.Where(x => x.VacancyStatusId == searchParams.VacancyStatusId);
            }

            if(searchParams.OfficeId.HasValue)
            {
                retVal = retVal.Where(x => x.OfficeId == searchParams.OfficeId.Value);
            }  
            else if(searchParams.CompanyId.HasValue)
            {
                retVal = retVal.Where(x => x.Office.CompanyId == searchParams.CompanyId.Value);
            }   

            if(searchParams.Skills.Any())
            {
                foreach(var skill in searchParams.Skills)
                {
                    retVal = retVal.Where(x => x.VacancySkills.Any(s => s.SkillId == skill.SkillId && s.SeniorityId == skill.SeniorityId));
                }
            }       

            return retVal.Include(x => x.Office)
                .ThenInclude(x => x.Company)
                .Include(x => x.VacancyStatus)
                .Include(x => x.CandidateOnVacancies)
                .ToList();
        }
    }

    public void Update(Vacancy vacancy)
    {
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (db.Vacancies.Any(x => x.Id == vacancy.Id))
                {
                    var entity = db.Vacancies.First(x => x.Id == vacancy.Id);
                    entity.LastModificationDate = DateTime.Now;
                    entity.Name = vacancy.Name;
                    entity.Description = vacancy.Description;
                    entity.VacancyStatusId = vacancy.VacancyStatusId;
                    entity.OfficeId = vacancy.OfficeId;
                    entity.Enabled = vacancy.Enabled;

                    //update existed resources
                    var idsToUpdate = vacancy.VacancyResources.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
                    foreach (var idToUpdate in idsToUpdate)
                    {
                        if (db.VacancyResources.Any(x => x.Id == idToUpdate))
                        {
                            var resourceToUpdate = vacancy.VacancyResources.First(x => x.Id == idToUpdate);
                            var resource = db.VacancyResources.First(x => x.Id == idToUpdate);
                            resource.Value = resourceToUpdate.Value;
                            resource.ResourceTypeId = resourceToUpdate.ResourceTypeId;
                        }
                    }
                    //delete existed resources
                    var idsToDelete = db.VacancyResources.Where(x => x.VacancyId == vacancy.Id && !idsToUpdate.Contains(x.Id))
                                                                   .Select(x => x.Id).ToArray();
                    foreach (var idToDelete in idsToDelete)
                    {
                        if (db.VacancyResources.Any(x => x.Id == idToDelete))
                        {
                            var resource = db.VacancyResources.First(x => x.Id == idToDelete);
                            db.VacancyResources.Remove(resource);
                        }
                    }
                    //add new resources
                    foreach (var resourceToAdd in vacancy.VacancyResources.Where(x => x.Id == 0))
                    {
                        var newResource = new VacancyResource
                        {
                            VacancyId = vacancy.Id,
                            Value = resourceToAdd.Value,
                            ResourceTypeId = resourceToAdd.ResourceTypeId
                        };
                        db.VacancyResources.Add(newResource);
                    }

                    //update existed skills
                    idsToUpdate = vacancy.VacancySkills.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
                    foreach (var idToUpdate in idsToUpdate)
                    {
                        if (db.VacancySkills.Any(x => x.Id == idToUpdate))
                        {
                            var skillToUpdate = vacancy.VacancySkills.First(x => x.Id == idToUpdate);
                            var skill = db.VacancySkills.First(x => x.Id == idToUpdate);
                            skill.SeniorityId = skillToUpdate.SeniorityId;
                            skill.SkillId = skillToUpdate.SkillId;
                        }
                    }
                    //delete existed skills
                    idsToDelete = db.VacancySkills.Where(x => x.VacancyId == vacancy.Id && !idsToUpdate.Contains(x.Id))
                                                                   .Select(x => x.Id).ToArray();
                    foreach (var idToDelete in idsToDelete)
                    {
                        if (db.VacancySkills.Any(x => x.Id == idToDelete))
                        {
                            var skill = db.VacancySkills.First(x => x.Id == idToDelete);
                            db.VacancySkills.Remove(skill);
                        }
                    }
                    //add new skills
                    foreach (var skillToAdd in vacancy.VacancySkills.Where(x => x.Id == 0))
                    {
                        var newSkill = new VacancySkill
                        {
                            VacancyId = vacancy.Id,
                            SeniorityId = skillToAdd.SeniorityId,
                            SkillId = skillToAdd.SkillId
                        };
                        db.VacancySkills.Add(newSkill);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
            }
        }
    }
}
