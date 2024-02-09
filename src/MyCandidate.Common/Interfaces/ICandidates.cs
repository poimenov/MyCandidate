namespace MyCandidate.Common.Interfaces
{
    public interface ICandidates
    {
        bool Exist(string lastName, string firstName, DateTime birthdate);
        Candidate Get(int id);
        void Delete(int id);
        bool Create(Candidate candidate, out int id);
        void Update(Candidate candidate);
        IEnumerable<Candidate> Search(CandidateSearch searchParams);
    } 

    public class CandidateSearch
    {
        public CandidateSearch()
        {
            Skills = new List<SkillValue>();
        }

        public CandidateSearch(List<VacancySkill> vacancySkills)
        {
            var skillsList = new List<SkillValue>();
            if(vacancySkills != null && vacancySkills.Any())
            {
                skillsList.AddRange(vacancySkills.Select(x => new SkillValue(x.SkillId, x.SeniorityId)));
            }
            
            Skills = skillsList;
        }

        public string LastName {get;set;}
        public string FirstName {get;set;}
        public IEnumerable<SkillValue> Skills {get;set;}
        public int? CountryId {get;set;}
        public int? CityId {get;set;}     
        public bool? Enabled {get;set;}    
    }  
}

