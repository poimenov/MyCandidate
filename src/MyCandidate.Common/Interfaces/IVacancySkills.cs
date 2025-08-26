namespace MyCandidate.Common.Interfaces;

public interface IVacancySkills
{
    Task<VacancySkill?> GetAsync(int id);
    Task<IEnumerable<VacancySkill>> GetVacancySkillsAsync(int vacancyId);
}
