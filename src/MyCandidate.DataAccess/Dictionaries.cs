using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Dictionaries : IDictionariesDataAccess
{
    private readonly IDatabaseFactory _databaseFactory;
    public Dictionaries(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }
    public IEnumerable<ResourceType> GetResourceTypes()
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.ResourceTypes.ToList();
        }
    }

    public IEnumerable<SelectionStatus> GetSelectionStatuses()
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.SelectionStatuses.ToList();
        }
    }

    public IEnumerable<Seniority> GetSeniorities()
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.Seniorities.ToList();
        }
    }

    public IEnumerable<VacancyStatus> GetVacancyStatuses()
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.VacancyStatuses.ToList();
        }
    }
}
