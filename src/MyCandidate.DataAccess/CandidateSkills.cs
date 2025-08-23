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
    public CandidateSkill Get(int id)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.CandidateSkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill!)
                .ThenInclude(x => x.SkillCategory)
                .First(x => x.Id == id);
        }
    }

    public IEnumerable<CandidateSkill> GetCandidateSkills(int candidateId)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.CandidateSkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill!)
                .ThenInclude(x => x.SkillCategory)
                .Where(x => x.CandidateId == candidateId)
                .ToList();
        }
    }
}
