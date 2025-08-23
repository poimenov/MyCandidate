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
    public IEnumerable<Comment> GetCommentsByCandidateId(int candidateId)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.Comments
                        .Where(x => x.CandidateOnVacancy!.CandidateId == candidateId)
                        .Include(x => x.CandidateOnVacancy!)
                        .ThenInclude(x => x.Vacancy)
                        .Include(x => x.CandidateOnVacancy!)
                        .ThenInclude(x => x.Candidate)
                        .ToList();
        }
    }

    public IEnumerable<Comment> GetCommentsByVacancyId(int vacancyId)
    {
        using (var db = _databaseFactory.CreateDbContext())
        {
            return db.Comments
                        .Where(x => x.CandidateOnVacancy!.VacancyId == vacancyId)
                        .Include(x => x.CandidateOnVacancy!)
                        .ThenInclude(x => x.Vacancy)
                        .Include(x => x.CandidateOnVacancy!)
                        .ThenInclude(x => x.Candidate)
                        .ToList();
        }
    }
}
