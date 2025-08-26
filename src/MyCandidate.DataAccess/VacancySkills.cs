using MyCandidate.Common;
using Microsoft.EntityFrameworkCore;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class VacancySkills : IVacancySkills
{
    private readonly IDatabaseFactory _databaseFactory;
    public VacancySkills(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<VacancySkill?> GetAsync(int id)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.VacancySkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill!)
                .ThenInclude(x => x.SkillCategory)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }

    public async Task<IEnumerable<VacancySkill>> GetVacancySkillsAsync(int vacancyId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.VacancySkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill!)
                .ThenInclude(x => x.SkillCategory)
                .Where(x => x.VacancyId == vacancyId)
                .ToListAsync();
        }
    }
}
