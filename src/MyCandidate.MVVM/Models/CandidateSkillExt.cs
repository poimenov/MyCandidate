using MyCandidate.Common;

namespace MyCandidate.MVVM.Models;

public class CandidateSkillExt : CandidateSkill
{
    public CandidateSkillExt(CandidateSkill candidateSkill)
    {
        Id = candidateSkill.Id;
        CandidateId = candidateSkill.CandidateId;
        Candidate = candidateSkill.Candidate;
        SkillId = candidateSkill.SkillId;
        Skill = candidateSkill.Skill;
        SeniorityId = candidateSkill.SeniorityId;
        Seniority = candidateSkill.Seniority;
    }
}
