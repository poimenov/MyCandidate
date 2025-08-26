using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class CandidateOnVacancies : ICandidateOnVacancies
{
    private readonly IDatabaseFactory _databaseFactory;
    public CandidateOnVacancies(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<IEnumerable<CandidateOnVacancy>> GetListByCandidateIdAsync(int candidateId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.CandidateOnVacancies
                        .Where(x => x.CandidateId == candidateId)
                        .Include(x => x.Candidate)
                        .Include(x => x.Vacancy)
                        .Include(x => x.SelectionStatus)
                        .ToListAsync();
        }
    }

    public async Task<IEnumerable<CandidateOnVacancy>> GetListByVacancyIdAsync(int vacancyId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.CandidateOnVacancies
                        .Where(x => x.VacancyId == vacancyId)
                        .Include(x => x.Candidate)
                        .Include(x => x.Vacancy)
                        .Include(x => x.SelectionStatus)
                        .ToListAsync();
        }
    }
}
