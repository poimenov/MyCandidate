namespace MyCandidate.Common.Interfaces;

public interface IVacancies
{
    Vacancy Get(int id);
    void Delete(int id);
    bool Create(Vacancy vacancy, out int id);
    void Update(Vacancy vacancy);    
}
