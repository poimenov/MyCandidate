using MyCandidate.Common;
using Microsoft.EntityFrameworkCore;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class CandidateSkills : ICandidateSkills
{
    private readonly IDatabaseFactory _databaseFactory;
    public CandidateSkills(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<CandidateSkill?> GetAsync(int id)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.CandidateSkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill!)
                .ThenInclude(x => x.SkillCategory)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }

    public async Task<IEnumerable<CandidateSkill>> GetCandidateSkillsAsync(int candidateId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.CandidateSkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill!)
                .ThenInclude(x => x.SkillCategory)
                .Where(x => x.CandidateId == candidateId)
                .ToListAsync();
        }
    }
}
