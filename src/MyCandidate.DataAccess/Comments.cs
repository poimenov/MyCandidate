using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Comments : IComments
{
    private readonly IDatabaseFactory _databaseFactory;
    public Comments(IDatabaseFactory databaseFactory)
    {
        _databaseFactory = databaseFactory;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByCandidateIdAsync(int candidateId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.Comments
                        .Where(x => x.CandidateOnVacancy!.CandidateId == candidateId)
                        .Include(x => x.CandidateOnVacancy!)
                        .ThenInclude(x => x.Vacancy)
                        .Include(x => x.CandidateOnVacancy!)
                        .ThenInclude(x => x.Candidate)
                        .ToListAsync();
        }
    }

    public async Task<IEnumerable<Comment>> GetCommentsByVacancyIdAsync(int vacancyId)
    {
        await using (var db = _databaseFactory.CreateDbContext())
        {
            return await db.Comments
                        .Where(x => x.CandidateOnVacancy!.VacancyId == vacancyId)
                        .Include(x => x.CandidateOnVacancy!)
                        .ThenInclude(x => x.Vacancy)
                        .Include(x => x.CandidateOnVacancy!)
                        .ThenInclude(x => x.Candidate)
                        .ToListAsync();
        }
    }
}
