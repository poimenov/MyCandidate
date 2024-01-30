using MyCandidate.Common;

namespace MyCandidate.MVVM.Services;

public interface IVacancyService
{
    Vacancy Get(int id);
    bool Delete(int id, out string message);
    bool Create (Vacancy item, out int id, out string message);
    bool Update (Vacancy item, out string message);    
}
