namespace MyCandidate.Common.Interfaces;

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
