using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess
{
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

                            var candidateOnVacancies = db.CandidateOnVacancies.Where(x => x.VacancyId == id).ToList();
                            candidateOnVacancies.ForEach(x => db.Comments.RemoveRange(db.Comments.Where(x => x.CandidateOnVacancyId == x.Id)));
                            db.CandidateOnVacancies.RemoveRange(candidateOnVacancies);

                            db.Vacancies.Remove(db.Vacancies.First(x => x.Id == id));

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
                var query = db.Vacancies.AsQueryable();
                query = query.Include(x => x.Office)
                    .ThenInclude(x => x.Company)
                    .Include(x => x.VacancyStatus)
                    .Include(x => x.VacancySkills)
                    .Include(x => x.CandidateOnVacancies);

                if (searchParams.Enabled.HasValue)
                {
                    query = query.Where(x => x.Enabled == searchParams.Enabled.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchParams.Name))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(searchParams.Name.ToLower()));
                }

                if (searchParams.VacancyStatusId.HasValue)
                {
                    query = query.Where(x => x.VacancyStatusId == searchParams.VacancyStatusId);
                }

                if (searchParams.OfficeId.HasValue)
                {
                    query = query.Where(x => x.OfficeId == searchParams.OfficeId.Value);
                }
                else if (searchParams.CompanyId.HasValue)
                {
                    query = query.Where(x => x.Office.CompanyId == searchParams.CompanyId.Value);
                }

                if (searchParams.Skills.Any())
                {
                    foreach (var skill in searchParams.Skills)
                    {
                        query = query.Where(x => x.VacancySkills.Any(s => s.SkillId == skill.SkillId));
                    }

                    return query.ToList().OrderDescending(new VacancySkillsComparer(searchParams.Skills));
                }

                return query.ToList();
            }
        }

        public IEnumerable<Vacancy> GetRecent(int count)
        {
            using (var db = new Database())
            {
                return db.Vacancies.OrderByDescending(x => x.LastModificationDate).Take(count).ToList();
            }            
        }

        public void Update(Vacancy vacancy)
        {
            using (var db = new Database())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var dateTime = DateTime.Now;
                        if (db.Vacancies.Any(x => x.Id == vacancy.Id))
                        {
                            var entity = db.Vacancies.First(x => x.Id == vacancy.Id);
                            entity.LastModificationDate = dateTime;
                            entity.Name = vacancy.Name;
                            entity.Description = vacancy.Description;
                            entity.VacancyStatusId = vacancy.VacancyStatusId;
                            entity.OfficeId = vacancy.OfficeId;
                            entity.Enabled = vacancy.Enabled;

                            ResourcesUpdate(db, vacancy);
                            SkillsUpdate(db, vacancy);
                            CandidateOnVacanciesUpdate(db, vacancy, dateTime);

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

        private void SkillsUpdate(Database db, Vacancy vacancy)
        {
            //update existed skills
            var idsToUpdate = vacancy.VacancySkills.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
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
            var idsToDelete = db.VacancySkills.Where(x => x.VacancyId == vacancy.Id && !idsToUpdate.Contains(x.Id))
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
        }

        private void ResourcesUpdate(Database db, Vacancy vacancy)
        {
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
        }

        private void CandidateOnVacanciesUpdate(Database db, Vacancy vacancy, DateTime dateTime)
        {
            //update existed candidateOnVacancies
            var idsToUpdate = vacancy.CandidateOnVacancies.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (db.CandidateOnVacancies.Any(x => x.Id == idToUpdate))
                {
                    var candidateOnVacancyToUpdate = vacancy.CandidateOnVacancies.First(x => x.Id == idToUpdate);
                    var candidateOnVacancy = db.CandidateOnVacancies.First(x => x.Id == idToUpdate);
                    candidateOnVacancy.SelectionStatusId = candidateOnVacancyToUpdate.SelectionStatusId;
                    candidateOnVacancy.LastModificationDate = dateTime;
                    CommentsUpdate(db, idToUpdate, candidateOnVacancyToUpdate.Comments, dateTime);
                }
            }
            //delete existed candidateOnVacancies
            var idsToDelete = db.CandidateOnVacancies.Where(x => x.VacancyId == vacancy.Id && !idsToUpdate.Contains(x.Id)).Select(x => x.Id).ToArray();
            foreach (var idToDelete in idsToDelete)
            {
                if (db.CandidateOnVacancies.Any(x => x.Id == idToDelete))
                {
                    db.Comments.RemoveRange(db.Comments.Where(x => x.CandidateOnVacancyId == idToDelete).ToList());
                    db.CandidateOnVacancies.Remove(db.CandidateOnVacancies.First(x => x.Id == idToDelete));
                }
            }
            //add new candidateOnVacancies
            foreach (var candidateOnVacancyToAdd in vacancy.CandidateOnVacancies.Where(x => x.Id <= 0))
            {
                var newCandidateOnVacancy = new CandidateOnVacancy
                {
                    CandidateId = candidateOnVacancyToAdd.CandidateId,
                    VacancyId = candidateOnVacancyToAdd.VacancyId,
                    SelectionStatusId = candidateOnVacancyToAdd.SelectionStatusId,
                    CreationDate = dateTime,
                    LastModificationDate = dateTime
                };
                db.CandidateOnVacancies.Add(newCandidateOnVacancy);
                db.SaveChanges();
                foreach (var comment in candidateOnVacancyToAdd.Comments)
                {
                    var newComment = new Comment
                    {
                        CandidateOnVacancyId = newCandidateOnVacancy.Id,
                        Value = comment.Value,
                        CreationDate = dateTime,
                        LastModificationDate = dateTime
                    };
                    db.Comments.Add(newComment);
                }
            }
        }

        private void CommentsUpdate(Database db, int candidateOnVacancyId, IEnumerable<Comment> comments, DateTime dateTime)
        {
            //update existed comments
            var idsToUpdate = comments.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (db.Comments.Any(x => x.Id == idToUpdate))
                {
                    var commentToUpdate = comments.First(x => x.Id == idToUpdate);
                    var comment = db.Comments.First(x => x.Id == idToUpdate);
                    comment.Value = commentToUpdate.Value;
                    comment.LastModificationDate = dateTime;
                }
            }
            //delete existed comments
            var idsToDelete = db.Comments.Where(x => x.CandidateOnVacancyId == candidateOnVacancyId && !idsToUpdate.Contains(x.Id)).Select(x => x.Id).ToArray();
            foreach (var idToDelete in idsToDelete)
            {
                if (db.Comments.Any(x => x.Id == idToDelete))
                {
                    db.Comments.Remove(db.Comments.First(x => x.Id == idToDelete));
                }
            }
            //add new comments
            foreach (var commentToAdd in comments.Where(x => x.Id <= 0))
            {
                var newComment = new Comment
                {
                    CandidateOnVacancyId = commentToAdd.CandidateOnVacancyId,
                    Value = commentToAdd.Value,
                    CreationDate = dateTime,
                    LastModificationDate = dateTime
                };
                db.Comments.Add(newComment);
            }
        }

    }

    internal class VacancySkillsComparer : IComparer<Vacancy>
    {
        private readonly IEnumerable<SkillValue> _skills;
        public VacancySkillsComparer(IEnumerable<SkillValue> skills)
        {
            _skills = skills;
        }
        public int Compare(Vacancy? x, Vacancy? y)
        {
            int xLength = x!.VacancySkills.Select(s => new SkillValue(s.SkillId, s.SeniorityId)).Intersect(_skills, new SkillValueComparer()).Count();
            int yLength = y!.VacancySkills.Select(s => new SkillValue(s.SkillId, s.SeniorityId)).Intersect(_skills, new SkillValueComparer()).Count();
            return xLength - yLength;
        }
    }
}
