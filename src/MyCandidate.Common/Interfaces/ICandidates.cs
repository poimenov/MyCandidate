namespace MyCandidate.Common.Interfaces
{
    public interface ICandidates
    {
        Task<bool> ExistAsync(int id);
        Task<bool> ExistAsync(string lastName, string firstName, DateTime? birthdate);
        Task<Candidate?> GetAsync(int id);
        Task<OperationResult> DeleteAsync(int id);
        Task<OperationResult<int>> CreateAsync(Candidate candidate);
        Task<OperationResult> UpdateAsync(Candidate candidate);
        Task<IEnumerable<Candidate>> SearchAsync(CandidateSearch searchParams);
        Task<IEnumerable<Candidate>> GetRecentAsync(int count);
    }

    public class CandidateSearch
    {
        public CandidateSearch()
        {
            Skills = new List<SkillValue>();
            SearchStrictBySeniority = true;
        }

        public CandidateSearch(List<VacancySkill> vacancySkills)
        {
            var skillsList = new List<SkillValue>();
            if (vacancySkills != null && vacancySkills.Any())
            {
                skillsList.AddRange(vacancySkills.Select(x => new SkillValue(x.SkillId, x.SeniorityId)));
            }

            Skills = skillsList;
            SearchStrictBySeniority = true;
        }

        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IEnumerable<SkillValue> Skills { get; set; }
        public bool SearchStrictBySeniority { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public bool? Enabled { get; set; }
    }
}

