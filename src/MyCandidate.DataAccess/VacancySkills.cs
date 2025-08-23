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

    public VacancySkill Get(int id)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.VacancySkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill!)
                .ThenInclude(x => x.SkillCategory)
                .First(x => x.Id == id);
        }
    }

    public IEnumerable<VacancySkill> GetVacancySkills(int vacancyId)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.VacancySkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill!)
                .ThenInclude(x => x.SkillCategory)
                .Where(x => x.VacancyId == vacancyId)
                .ToList();
        }
    }
}
