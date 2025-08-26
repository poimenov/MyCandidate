namespace MyCandidate.Common.Interfaces;

public interface ICandidateSkills
{
    Task<CandidateSkill?> GetAsync(int id);
    Task<IEnumerable<CandidateSkill>> GetCandidateSkillsAsync(int candidateId);
}
