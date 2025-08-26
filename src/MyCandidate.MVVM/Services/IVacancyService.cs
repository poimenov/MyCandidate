using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public interface IVacancyService
{
    Task<bool> ExistAsync(int id);
    Task<Vacancy?> GetAsync(int id);
    Task<XmlDocument> GetXmlAsync(int id);
    Task<OperationResult> DeleteAsync(int id);
    Task<OperationResult<int>> CreateAsync(Vacancy item);
    Task<OperationResult> UpdateAsync(Vacancy item);
    Task<IEnumerable<Vacancy>> SearchAsync(VacancySearch searchParams);
    Task<IEnumerable<Vacancy>> GetRecentasync(int count);
    Task<IEnumerable<CandidateOnVacancy>> GetCandidateOnVacanciesAsync(int vacancyId);
    Task<IEnumerable<Comment>> GetCommentsAsync(int vacancyId);
}
