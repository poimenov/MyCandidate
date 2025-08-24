namespace MyCandidate.Common.Interfaces;

public interface IDictionariesDataAccess
{
    Task<IEnumerable<ResourceType>> GetResourceTypesAsync();
    Task<IEnumerable<SelectionStatus>> GetSelectionStatusesAsync();
    Task<IEnumerable<VacancyStatus>> GetVacancyStatusesAsync();
    Task<IEnumerable<Seniority>> GetSenioritiesAsync();
}
