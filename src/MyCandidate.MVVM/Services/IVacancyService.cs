using System.Collections.Generic;
using System.Xml;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public interface IVacancyService
{
    bool Exist(int id);
    Vacancy Get(int id);
    XmlDocument GetXml(int id);
    bool Delete(int id, out string message);
    bool Create (Vacancy item, out int id, out string message);
    bool Update (Vacancy item, out string message);   
    IEnumerable<Vacancy> Search(VacancySearch searchParams); 
    IEnumerable<Vacancy> GetRecent(int count);
    IEnumerable<CandidateOnVacancy> GetCandidateOnVacancies(int vacancyId);
    IEnumerable<Comment> GetComments(int vacancyId);
}
