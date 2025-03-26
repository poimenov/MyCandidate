namespace MyCandidate.Common.Interfaces
{
    public interface IVacancies
    {
        bool Exist(int id);
        Vacancy Get(int id);
        void Delete(int id);
        bool Create(Vacancy vacancy, out int id);
        void Update(Vacancy vacancy);
        IEnumerable<Vacancy> Search(VacancySearch searchParams);
        IEnumerable<Vacancy> GetRecent(int count);
    }

    public class VacancySearch
    {
        public VacancySearch()
        {
            Skills = new List<SkillValue>();
            SearchStrictBySeniority = true;
        }

        public VacancySearch(List<CandidateSkill> candidateSkills)
        {
            var skillsList = new List<SkillValue>();
            if (candidateSkills != null && candidateSkills.Any())
            {
                skillsList.AddRange(candidateSkills.Select(x => new SkillValue(x.SkillId, x.SeniorityId)));
            }

            Skills = skillsList;
            SearchStrictBySeniority = true;
        }

        public string Name { get; set; } = string.Empty;
        public int? VacancyStatusId { get; set; }
        public IEnumerable<SkillValue> Skills { get; set; }
        public bool SearchStrictBySeniority { get; set; }
        public int? CompanyId { get; set; }
        public int? OfficeId { get; set; }
        public bool? Enabled { get; set; }
    }

}

