using MyCandidate.Common;
using Microsoft.EntityFrameworkCore;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class CandidateSkills : ICandidateSkills
{
    public CandidateSkill Get(int id)
    {
        using (var db = new Database())
        {
            return db.CandidateSkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill)
                .ThenInclude(x => x.SkillCategory)
                .First(x => x.Id == id);
        }
    }

    public IEnumerable<CandidateSkill> GetCandidateSkills(int candidateId)
    {
        using (var db = new Database())
        {
            return db.CandidateSkills
                .Include(x => x.Seniority)
                .Include(x => x.Skill)
                .ThenInclude(x => x.SkillCategory)
                .Where(x => x.CandidateId == candidateId)
                .ToList();
        }
    }
}
