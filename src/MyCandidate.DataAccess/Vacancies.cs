using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Vacancies : IDataAccess<Vacancy>
{
    public Vacancies()
    {
        //
    }
    
    public IEnumerable<Vacancy> ItemsList
    {
        get
        {
            using (var db = new Database())
            {
                return db.Vacancies
                    .Include(x => x.Office)
                    .Include(x => x.VacancySkills)
                    .Include(x => x.CandidateOnVacancies)
                    .Include(x => x.VacancyStatus)
                    .ToList();
            }
        }
    }    

    public void Create(IEnumerable<Vacancy> items)
    {
        throw new NotImplementedException();
    }

    public void Delete(IEnumerable<int> itemIds)
    {
        throw new NotImplementedException();
    }

    public Vacancy? Get(int itemId)
    {
        throw new NotImplementedException();
    }

    public void Update(IEnumerable<Vacancy> items)
    {
        throw new NotImplementedException();
    }
}
