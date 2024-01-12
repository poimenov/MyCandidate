using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Dictionaries : IDictionariesDataAccess
{  
    public IEnumerable<ResourceType> GetResourceTypes()
    {
        using (var db = new Database())
        {
            return db.ResourceTypes.ToList();
        }
    }

    public IEnumerable<SelectionStatus> GetSelectionStatuses()
    {
        using (var db = new Database())
        {
            return db.SelectionStatuses.ToList();
        }
    }

    public IEnumerable<Seniority> GetSeniorities()
    {
        using (var db = new Database())
        {
            return db.Seniorities.ToList();
        }
    }

    public IEnumerable<VacancyStatus> GetVacancyStatuses()
    {
        using (var db = new Database())
        {
            return db.VacancyStatuses.ToList();
        }
    }
}
