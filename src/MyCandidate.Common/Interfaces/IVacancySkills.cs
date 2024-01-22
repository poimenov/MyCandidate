namespace MyCandidate.Common.Interfaces;

public interface IVacancySkills
{
    VacancySkill Get(int id);
    IEnumerable<VacancySkill> GetVacancySkills(int vacancyId);    
}
