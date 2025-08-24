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

    public async Task<IEnumerable<ResourceType>> GetResourceTypesAsync()
    {
        await using var db = _databaseFactory.CreateDbContext();
        return await db.ResourceTypes.ToListAsync();
    }

    public async Task<IEnumerable<SelectionStatus>> GetSelectionStatusesAsync()
    {
        await using var db = _databaseFactory.CreateDbContext();
        return await db.SelectionStatuses.ToListAsync();
    }

    public async Task<IEnumerable<Seniority>> GetSenioritiesAsync()
    {
        await using var db = _databaseFactory.CreateDbContext();
        return await db.Seniorities.ToListAsync();
    }

    public async Task<IEnumerable<VacancyStatus>> GetVacancyStatusesAsync()
    {
        await using var db = _databaseFactory.CreateDbContext();
        return await db.VacancyStatuses.ToListAsync();
    }
}
