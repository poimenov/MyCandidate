using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess
{
    public class Candidates : ICandidates
    {
        private readonly IDatabaseFactory _databaseFactory;
        public Candidates(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public async Task<bool> ExistAsync(int id)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                return await db.Candidates.AnyAsync(x => x.Id == id);
            }
        }

        public async Task<bool> ExistAsync(string lastName, string firstName, DateTime? birthdate)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                return await db.Candidates.AnyAsync(x => x.LastName.Trim().ToLower() == lastName.Trim().ToLower()
                                                && x.FirstName.Trim().ToLower() == firstName.Trim().ToLower()
                                                && x.BirthDate == birthdate);
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            var retVal = new OperationResult { Success = false };
            await using (var db = _databaseFactory.CreateDbContext())
            {
                await using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    if (await db.Candidates.AnyAsync(x => x.Id == id))
                    {
                        try
                        {
                            if (await db.CandidateResources.AnyAsync(x => x.CandidateId == id))
                            {
                                db.CandidateResources.RemoveRange(db.CandidateResources.Where(x => x.CandidateId == id));
                            }

                            if (await db.CandidateSkills.AnyAsync(x => x.CandidateId == id))
                            {
                                db.CandidateSkills.RemoveRange(db.CandidateSkills.Where(x => x.CandidateId == id));
                            }

                            var item = await db.Candidates.FirstAsync(x => x.Id == id);
                            db.Locations.Remove(await db.Locations.FirstAsync(x => x.Id == item.LocationId));

                            var candidateOnVacancies = db.CandidateOnVacancies.Where(x => x.CandidateId == id).ToList();
                            foreach (var cov in candidateOnVacancies)
                            {
                                db.Comments.RemoveRange(db.Comments.Where(x => x.CandidateOnVacancyId == cov.Id));
                            }
                            db.CandidateOnVacancies.RemoveRange(candidateOnVacancies);

                            db.Candidates.Remove(item);

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
                    else
                    {
                        retVal.Message = "Candidate not found.";
                    }
                }
            }
            return retVal;
        }

        public async Task<OperationResult<int>> CreateAsync(Candidate candidate)
        {
            var retVal = new OperationResult<int> { Success = false, Result = 0 };

            if (await ExistAsync(candidate.LastName, candidate.FirstName, candidate.BirthDate))
            {
                retVal.Message = "Candidate already exists.";
                return retVal;
            }

            await using (var db = _databaseFactory.CreateDbContext())
            {
                await using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var newLocation = new Location
                        {
                            Address = candidate.Location!.Address ?? string.Empty,
                            CityId = candidate.Location!.CityId
                        };
                        await db.Locations.AddAsync(newLocation);
                        await db.SaveChangesAsync();

                        var newCandidate = new Candidate
                        {
                            LastName = candidate.LastName,
                            FirstName = candidate.FirstName,
                            Title = candidate.Title,
                            BirthDate = candidate.BirthDate,
                            LocationId = newLocation.Id,
                            CreationDate = DateTime.Now,
                            LastModificationDate = DateTime.Now
                        };
                        await db.Candidates.AddAsync(newCandidate);
                        await db.SaveChangesAsync();
                        var id = newCandidate.Id;

                        foreach (var candidateResource in candidate.CandidateResources)
                        {
                            var newCandidateResource = new CandidateResource
                            {
                                CandidateId = id,
                                Value = candidateResource.Value,
                                ResourceTypeId = candidateResource.ResourceTypeId
                            };
                            await db.CandidateResources.AddAsync(newCandidateResource);
                        }

                        foreach (var candidateSkill in candidate.CandidateSkills)
                        {
                            var newCandidateSkill = new CandidateSkill
                            {
                                CandidateId = id,
                                SeniorityId = candidateSkill.SeniorityId,
                                SkillId = candidateSkill.SkillId
                            };
                            await db.CandidateSkills.AddAsync(newCandidateSkill);
                        }
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        retVal.Success = true;
                        retVal.Result = id;

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

        public async Task<OperationResult> UpdateAsync(Candidate candidate)
        {
            var retVal = new OperationResult { Success = false };
            await using (var db = _databaseFactory.CreateDbContext())
            {
                await using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var dateTime = DateTime.Now;
                        if (await db.Candidates.AnyAsync(x => x.Id == candidate.Id))
                        {
                            var entity = await db.Candidates.FirstAsync(x => x.Id == candidate.Id);
                            entity.LastModificationDate = dateTime;
                            entity.FirstName = candidate.FirstName;
                            entity.LastName = candidate.LastName;
                            entity.Title = candidate.Title;
                            entity.BirthDate = candidate.BirthDate;
                            entity.Enabled = candidate.Enabled;

                            var location = await db.Locations.FirstAsync(x => x.Id == entity.LocationId);
                            location.Address = candidate.Location!.Address;
                            location.CityId = candidate.Location.CityId;
                            await ResourcesUpdateAsync(db, candidate);
                            await SkillsUpdateAsync(db, candidate);
                            await CandidateOnVacanciesUpdateAsync(db, candidate, dateTime);
                            await db.SaveChangesAsync();
                            await transaction.CommitAsync();
                            retVal.Success = true;
                        }
                        else
                        {
                            retVal.Message = "Candidate not found.";
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

        public async Task<Candidate?> GetAsync(int id)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                return await db.Candidates
                    .Include(x => x.Location!)
                    .ThenInclude(x => x.City!)
                    .ThenInclude(x => x.Country)
                    .Include(x => x.CandidateSkills)
                    .ThenInclude(x => x.Seniority)
                    .Include(x => x.CandidateSkills)
                    .ThenInclude(x => x.Skill!)
                    .ThenInclude(x => x.SkillCategory)
                    .Include(x => x.CandidateResources)
                    .ThenInclude(x => x.ResourceType)
                    .Include(x => x.CandidateOnVacancies)
                    .ThenInclude(x => x.Vacancy)
                    .Include(x => x.CandidateOnVacancies)
                    .ThenInclude(x => x.SelectionStatus)
                    .Include(x => x.CandidateOnVacancies)
                    .ThenInclude(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<IEnumerable<Candidate>> GetRecentAsync(int count)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                return await db.Candidates
                    .OrderByDescending(x => x.LastModificationDate)
                    .Take(count)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Candidate>> SearchAsync(CandidateSearch searchParams)
        {
            await using (var db = _databaseFactory.CreateDbContext())
            {
                var query = db.Candidates.AsQueryable();
                query = query.Include(x => x.Location!)
                        .ThenInclude(x => x.City!)
                        .ThenInclude(x => x.Country)
                        .Include(x => x.CandidateSkills)
                        .Include(x => x.CandidateOnVacancies);

                if (searchParams.Enabled.HasValue)
                {
                    query = query.Where(x => x.Enabled == searchParams.Enabled.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchParams.LastName))
                {
                    query = query.Where(x => x.LastName.ToLower().StartsWith(searchParams.LastName.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(searchParams.FirstName))
                {
                    query = query.Where(x => x.FirstName.ToLower().StartsWith(searchParams.FirstName.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(searchParams.Title))
                {
                    query = query.Where(x => x.Title!.ToLower().Contains(searchParams.Title.ToLower()));
                }

                if (searchParams.CityId.HasValue)
                {
                    query = query.Where(x => x.Location!.CityId == searchParams.CityId.Value);
                }
                else if (searchParams.CountryId.HasValue)
                {
                    query = query.Where(x => x.Location!.City!.CountryId == searchParams.CountryId.Value);
                }

                if (searchParams.Skills.Any())
                {
                    foreach (var skill in searchParams.Skills)
                    {
                        if (searchParams.SearchStrictBySeniority)
                        {
                            query = query.Where(x => x.CandidateSkills.Any(s => s.SkillId == skill.SkillId && s.SeniorityId == skill.SeniorityId));
                        }
                        else
                        {
                            query = query.Where(x => x.CandidateSkills.Any(s => s.SkillId == skill.SkillId));
                        }
                    }

                    if (!searchParams.SearchStrictBySeniority)
                    {
                        var candidates = await query.ToListAsync();
                        candidates.Sort(new CandidateSkillsComparer(searchParams.Skills));
                        candidates.Reverse();
                        return candidates;
                    }
                }

                return await query.ToListAsync();
            }
        }

        private async Task CandidateOnVacanciesUpdateAsync(Database db, Candidate candidate, DateTime dateTime)
        {
            //update existed candidateOnVacancies
            var idsToUpdate = candidate.CandidateOnVacancies.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (await db.CandidateOnVacancies.AnyAsync(x => x.Id == idToUpdate))
                {
                    var candidateOnVacancyToUpdate = candidate.CandidateOnVacancies.First(x => x.Id == idToUpdate);
                    var candidateOnVacancy = db.CandidateOnVacancies.First(x => x.Id == idToUpdate);
                    candidateOnVacancy.SelectionStatusId = candidateOnVacancyToUpdate.SelectionStatusId;
                    candidateOnVacancy.LastModificationDate = dateTime;
                    await CommentsUpdateAsync(db, idToUpdate, candidateOnVacancyToUpdate.Comments, dateTime);
                }
            }
            //delete existed candidateOnVacancies
            var idsToDelete = await db.CandidateOnVacancies
                .Where(x => x.CandidateId == candidate.Id
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
            foreach (var candidateOnVacancyToAdd in candidate.CandidateOnVacancies.Where(x => x.Id <= 0))
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

        private async Task SkillsUpdateAsync(Database db, Candidate candidate)
        {
            //update existed skills
            var idsToUpdate = candidate.CandidateSkills.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (await db.CandidateSkills.AnyAsync(x => x.Id == idToUpdate))
                {
                    var skillToUpdate = candidate.CandidateSkills.First(x => x.Id == idToUpdate);
                    var skill = await db.CandidateSkills.FirstAsync(x => x.Id == idToUpdate);
                    skill.SeniorityId = skillToUpdate.SeniorityId;
                    skill.SkillId = skillToUpdate.SkillId;
                }
            }
            //delete existed skills
            var idsToDelete = await db.CandidateSkills
                .Where(x => x.CandidateId == candidate.Id && !idsToUpdate.Contains(x.Id))
                .Select(x => x.Id).ToArrayAsync();

            foreach (var idToDelete in idsToDelete)
            {
                if (await db.CandidateSkills.AnyAsync(x => x.Id == idToDelete))
                {
                    var skill = await db.CandidateSkills.FirstAsync(x => x.Id == idToDelete);
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
                await db.CandidateSkills.AddAsync(newSkill);
            }
        }

        private async Task ResourcesUpdateAsync(Database db, Candidate candidate)
        {
            //update existed resources
            var idsToUpdate = candidate.CandidateResources.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            foreach (var idToUpdate in idsToUpdate)
            {
                if (await db.CandidateResources.AnyAsync(x => x.Id == idToUpdate))
                {
                    var resourceToUpdate = candidate.CandidateResources.First(x => x.Id == idToUpdate);
                    var resource = await db.CandidateResources.FirstAsync(x => x.Id == idToUpdate);
                    resource.Value = resourceToUpdate.Value;
                    resource.ResourceTypeId = resourceToUpdate.ResourceTypeId;
                }
            }
            //delete existed resources
            var idsToDelete = await db.CandidateResources
                .Where(x => x.CandidateId == candidate.Id
                    && !idsToUpdate.Contains(x.Id))
                .Select(x => x.Id).ToArrayAsync();

            foreach (var idToDelete in idsToDelete)
            {
                if (await db.CandidateResources.AnyAsync(x => x.Id == idToDelete))
                {
                    var resource = await db.CandidateResources.FirstAsync(x => x.Id == idToDelete);
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
                await db.CandidateResources.AddAsync(newResource);
            }
        }

        private async Task CommentsUpdateAsync(Database db, int candidateOnVacancyId,
            IEnumerable<Comment> comments, DateTime dateTime)
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
                await db.Comments.AddAsync(newComment);
            }
        }
    }

    internal class CandidateSkillsComparer : IComparer<Candidate>
    {
        private readonly IEnumerable<SkillValue> _skills;
        public CandidateSkillsComparer(IEnumerable<SkillValue> skills)
        {
            _skills = skills;
        }
        public int Compare(Candidate? x, Candidate? y)
        {
            int xLength = x!.CandidateSkills.Select(s => new SkillValue(s.SkillId, s.SeniorityId)).Intersect(_skills, new SkillValueComparer()).Count();
            int yLength = y!.CandidateSkills.Select(s => new SkillValue(s.SkillId, s.SeniorityId)).Intersect(_skills, new SkillValueComparer()).Count();
            return xLength - yLength;
        }
    }
}