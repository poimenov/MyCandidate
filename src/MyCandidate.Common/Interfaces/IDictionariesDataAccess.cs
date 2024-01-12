namespace MyCandidate.Common.Interfaces;

public interface IDictionariesDataAccess
{
    IEnumerable<ResourceType> GetResourceTypes();
    IEnumerable<SelectionStatus> GetSelectionStatuses();
    IEnumerable<VacancyStatus> GetVacancyStatuses();
    IEnumerable<Seniority> GetSeniorities();
}
