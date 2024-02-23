using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class CandidateOnVacancies : ICandidateOnVacancies
{
    public CandidateOnVacancies()
    {
        //
    }

    public IEnumerable<CandidateOnVacancy> GetListByCandidateId(int candidateId)
    {
        using (var db = new Database())
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
        using (var db = new Database())
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
