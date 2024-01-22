using MyCandidate.Common;

namespace MyCandidate.MVVM.Services;

public interface IVacancyService
{
    Vacancy Get(int id);
    bool Create (Vacancy item, out string message);
    bool Update (Vacancy item, out string message);    
}
