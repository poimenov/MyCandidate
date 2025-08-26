using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess
{
    public class Vacancies : IVacancies
    {
        private readonly IDatabaseFactory _databaseFactory;
        public Vacancies(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public async Task<bool> ExistAsync(int id)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                return await db.Vacancies.AnyAsync(x => x.Id == id);
            }
        }

        public async Task<OperationResult<int>> CreateAsync(Vacancy vacancy)
        {
            var operationResult = new OperationResult<int> { Success = false, Result = 0 };
            await using (var db = _databaseFactory.CreateDbContext())
            {
                await using (var transaction = await db.Database.BeginTransactionAsync())
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
                        await db.Vacancies.AddAsync(newVacancy);
                        await db.SaveChangesAsync();
                        operationResult.Result = newVacancy.Id;

                        foreach (var VacancyResource in vacancy.VacancyResources)
                        {
                            var newVacancyResource = new VacancyResource
                            {
                                VacancyId = operationResult.Result,
                                Value = VacancyResource.Value,
                                ResourceTypeId = VacancyResource.ResourceTypeId
                            };
                            await db.VacancyResources.AddAsync(newVacancyResource);
                        }
                        foreach (var VacancySkill in vacancy.VacancySkills)
                        {
                            var newVacancySkill = new VacancySkill
                            {
                                VacancyId = operationResult.Result,
                                SeniorityId = VacancySkill.SeniorityId,
                                SkillId = VacancySkill.SkillId
                            };
                            await db.VacancySkills.AddAsync(newVacancySkill);
                        }
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        operationResult.Success = true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        operationResult.Message = ex.Message;
                        operationResult.Exception = ex;
                    }
                }
            }
            return operationResult;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var retVal = new OperationResult { Success = false };
            await using (var db = _databaseFactory.CreateDbContext())
            {
                await using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    if (await db.Vacancies.AnyAsync(x => x.Id == id))
                    {
                        try
                        {
                            if (await db.VacancyResources.AnyAsync(x => x.VacancyId == id))
                            {
                                db.VacancyResources.RemoveRange(db.VacancyResources.Where(x => x.VacancyId == id));
                            }

                            if (await db.VacancySkills.AnyAsync(x => x.VacancyId == id))
                            {
                                db.VacancySkills.RemoveRange(db.VacancySkills.Where(x => x.VacancyId == id));
                            }

                            var candidateOnVacancies = await db.CandidateOnVacancies.Where(x => x.VacancyId == id).ToListAsync();
                            foreach (var cov in candidateOnVacancies)
                            {
                                db.Comments.RemoveRange(db.Comments.Where(x => x.CandidateOnVacancyId == cov.Id));
                            }
                            db.CandidateOnVacancies.RemoveRange(candidateOnVacancies);

                            db.Vacancies.Remove(await db.Vacancies.FirstAsync(x => x.Id == id));

                            await db.SaveChangesAsync();
                            await transaction.CommitAsync();
                            retVal.Success = true;
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            retVal.Message = ex.Message;
                            retVal.Exception = ex;
                        }
                    }
                }
            }

            return retVal;
        }

        public async Task<Vacancy?> GetAsync(int id)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                return await db.Vacancies
                    .Include(x => x.Office!)
                    .ThenInclude(x => x.Company)
                    .Include(x => x.Office!)
                    .ThenInclude(x => x.Location!)
                    .ThenInclude(x => x.City!)
                    .ThenInclude(x => x.Country)
                    .Include(x => x.VacancyStatus)
                    .Include(x => x.VacancyResources)
                    .ThenInclude(x => x.ResourceType)
                    .Include(x => x.VacancySkills)
                    .ThenInclude(x => x.Seniority)
                    .Include(x => x.VacancySkills)
                    .ThenInclude(x => x.Skill!)
                    .ThenInclude(x => x.SkillCategory)
                    .Include(x => x.CandidateOnVacancies)
                    .ThenInclude(x => x.SelectionStatus)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<IEnumerable<Vacancy>> SearchAsync(VacancySearch searchParams)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                var query = db.Vacancies.AsQueryable();
                query = query.Include(x => x.Office!)
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
                    query = query.Where(x => x.Office!.CompanyId == searchParams.CompanyId.Value);
                }

                if (searchParams.Skills.Any())
                {
                    foreach (var skill in searchParams.Skills)
                    {
                        if (searchParams.SearchStrictBySeniority)
                        {
                            query = query.Where(x => x.VacancySkills.Any(s => s.SkillId == skill.SkillId && s.SeniorityId == skill.SeniorityId));
                        }
                        else
                        {
                            query = query.Where(x => x.VacancySkills.Any(s => s.SkillId == skill.SkillId));
                        }
                    }

                    if (!searchParams.SearchStrictBySeniority)
                    {
                        return (await query.ToListAsync()).OrderDescending(new VacancySkillsComparer(searchParams.Skills));
                    }
                }

                return await query.ToListAsync();
            }
        }

        public async Task<IEnumerable<Vacancy>> GetRecentAsync(int count)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                return await db.Vacancies
                    .OrderByDescending(x => x.LastModificationDate)
                    .Take(count)
                    .ToListAsync();
            }
        }

        public async Task<OperationResult> UpdateAsync(Vacancy vacancy)
        {
            var retVal = new OperationResult { Success = false };
            await using (var db = _databaseFactory.CreateDbContext())
            {
                await using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var dateTime = DateTime.Now;
                        if (await db.Vacancies.AnyAsync(x => x.Id == vacancy.Id))
                        {
                            var entity = await db.Vacancies.FirstAsync(x => x.Id == vacancy.Id);
                            entity.LastModificationDate = dateTime;
                            entity.Name = vacancy.Name;
                            entity.Description = vacancy.Description;
                            entity.VacancyStatusId = vacancy.VacancyStatusId;
                            entity.OfficeId = vacancy.OfficeId;
                            entity.Enabled = vacancy.Enabled;

                            await ResourcesUpdate(db, vacancy);
                            await SkillsUpdate(db, vacancy);
                            await CandidateOnVacanciesUpdate(db, vacancy, dateTime);

                            await db.SaveChangesAsync();
                            await transaction.CommitAsync();
                            retVal.Success = true;
                        }
                        else
                        {
                            retVal.Message = "Vacancy not found.";
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        retVal.Message = ex.Message;
                        retVal.Exception = ex;
                    }
                }
            }
            return retVal;
        }

        private async Task SkillsUpdate(Database db, Vacancy vacancy)
        {
            //update existed skills
            var idsToUpdate = vacancy.VacancySkills.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (await db.VacancySkills.AnyAsync(x => x.Id == idToUpdate))
                {
                    var skillToUpdate = vacancy.VacancySkills.First(x => x.Id == idToUpdate);
                    var skill = await db.VacancySkills.FirstAsync(x => x.Id == idToUpdate);
                    skill.SeniorityId = skillToUpdate.SeniorityId;
                    skill.SkillId = skillToUpdate.SkillId;
                }
            }
            //delete existed skills
            var idsToDelete = await db.VacancySkills
                .Where(x => x.VacancyId == vacancy.Id && !idsToUpdate.Contains(x.Id))
                .Select(x => x.Id).ToArrayAsync();

            foreach (var idToDelete in idsToDelete)
            {
                if (await db.VacancySkills.AnyAsync(x => x.Id == idToDelete))
                {
                    var skill = await db.VacancySkills.FirstAsync(x => x.Id == idToDelete);
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
                await db.VacancySkills.AddAsync(newSkill);
            }
        }

        private async Task ResourcesUpdate(Database db, Vacancy vacancy)
        {
            //update existed resources
            var idsToUpdate = vacancy.VacancyResources.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (await db.VacancyResources.AnyAsync(x => x.Id == idToUpdate))
                {
                    var resourceToUpdate = vacancy.VacancyResources.First(x => x.Id == idToUpdate);
                    var resource = await db.VacancyResources.FirstAsync(x => x.Id == idToUpdate);
                    resource.Value = resourceToUpdate.Value;
                    resource.ResourceTypeId = resourceToUpdate.ResourceTypeId;
                }
            }
            //delete existed resources
            var idsToDelete = await db.VacancyResources
                .Where(x => x.VacancyId == vacancy.Id && !idsToUpdate.Contains(x.Id))
                .Select(x => x.Id).ToArrayAsync();

            foreach (var idToDelete in idsToDelete)
            {
                if (await db.VacancyResources.AnyAsync(x => x.Id == idToDelete))
                {
                    db.VacancyResources.Remove(await db.VacancyResources.FirstAsync(x => x.Id == idToDelete));
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
                await db.VacancyResources.AddAsync(newResource);
            }
        }

        private async Task CandidateOnVacanciesUpdate(Database db, Vacancy vacancy, DateTime dateTime)
        {
            //update existed candidateOnVacancies
            var idsToUpdate = vacancy.CandidateOnVacancies.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (await db.CandidateOnVacancies.AnyAsync(x => x.Id == idToUpdate))
                {
                    var candidateOnVacancyToUpdate = vacancy.CandidateOnVacancies.First(x => x.Id == idToUpdate);
                    var candidateOnVacancy = await db.CandidateOnVacancies.FirstAsync(x => x.Id == idToUpdate);
                    candidateOnVacancy.SelectionStatusId = candidateOnVacancyToUpdate.SelectionStatusId;
                    candidateOnVacancy.LastModificationDate = dateTime;
                    await CommentsUpdate(db, idToUpdate, candidateOnVacancyToUpdate.Comments, dateTime);
                }
            }
            //delete existed candidateOnVacancies
            var idsToDelete = await db.CandidateOnVacancies
                .Where(x => x.VacancyId == vacancy.Id
                    && !idsToUpdate.Contains(x.Id))
                .Select(x => x.Id).ToArrayAsync();

            foreach (var idToDelete in idsToDelete)
            {
                if (await db.CandidateOnVacancies.AnyAsync(x => x.Id == idToDelete))
                {
                    db.Comments.RemoveRange(await db.Comments.Where(x => x.CandidateOnVacancyId == idToDelete).ToListAsync());
                    db.CandidateOnVacancies.Remove(await db.CandidateOnVacancies.FirstAsync(x => x.Id == idToDelete));
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
                await db.CandidateOnVacancies.AddAsync(newCandidateOnVacancy);
                await db.SaveChangesAsync();
                foreach (var comment in candidateOnVacancyToAdd.Comments)
                {
                    var newComment = new Comment
                    {
                        CandidateOnVacancyId = newCandidateOnVacancy.Id,
                        Value = comment.Value,
                        CreationDate = dateTime,
                        LastModificationDate = dateTime
                    };
                    await db.Comments.AddAsync(newComment);
                }
            }
        }

        private async Task CommentsUpdate(Database db, int candidateOnVacancyId, IEnumerable<Comment> comments, DateTime dateTime)
        {
            //update existed comments
            var idsToUpdate = comments.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (await db.Comments.AnyAsync(x => x.Id == idToUpdate))
                {
                    var commentToUpdate = comments.First(x => x.Id == idToUpdate);
                    var comment = await db.Comments.FirstAsync(x => x.Id == idToUpdate);
                    comment.Value = commentToUpdate.Value;
                    comment.LastModificationDate = dateTime;
                }
            }
            //delete existed comments
            var idsToDelete = await db.Comments
                .Where(x => x.CandidateOnVacancyId == candidateOnVacancyId
                    && !idsToUpdate.Contains(x.Id))
                .Select(x => x.Id).ToArrayAsync();

            foreach (var idToDelete in idsToDelete)
            {
                if (await db.Comments.AnyAsync(x => x.Id == idToDelete))
                {
                    db.Comments.Remove(await db.Comments.FirstAsync(x => x.Id == idToDelete));
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
                await db.Comments.AddAsync(newComment);
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
