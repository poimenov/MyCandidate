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

    public IEnumerable<CandidateOnVacancy> GetListByCandidateId(int candidateId)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.CandidateOnVacancies
                        .Where(x => x.CandidateId == candidateId)
                        .Include(x => x.Candidate)
                        .Include(x => x.Vacancy)
                        .Include(x => x.SelectionStatus)
                        .ToList();
        }
    }

    public IEnumerable<CandidateOnVacancy> GetListByVacancyId(int vacancyId)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.CandidateOnVacancies
                        .Where(x => x.VacancyId == vacancyId)
                        .Include(x => x.Candidate)
                        .Include(x => x.Vacancy)
                        .Include(x => x.SelectionStatus)
                        .ToList();
        }
    }
}
