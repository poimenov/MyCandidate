using System.Diagnostics.CodeAnalysis;

namespace MyCandidate.Common.Interfaces
{
    public struct SkillValue
    {
        public SkillValue(int skillId, int seniorityId)
        {
            SkillId = skillId;
            SeniorityId = seniorityId;
        }

        public int SkillId { get; set; } 
        public int SeniorityId { get; set; }  
    }

    public class SkillValueComparer : IEqualityComparer<SkillValue>
    {
        public bool Equals(SkillValue x, SkillValue y) => x.SkillId == y.SkillId && x.SeniorityId == y.SeniorityId;

        public int GetHashCode([DisallowNull] SkillValue obj) => HashCode.Combine(obj.SkillId.GetHashCode(), obj.SeniorityId.GetHashCode());
    }
}

