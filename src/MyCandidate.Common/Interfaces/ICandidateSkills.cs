namespace MyCandidate.Common.Interfaces;

public interface ICandidateSkills
{
    CandidateSkill Get(int id);
    IEnumerable<CandidateSkill> GetCandidateSkills(int candidateId);
}
