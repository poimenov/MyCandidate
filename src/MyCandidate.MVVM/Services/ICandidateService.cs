using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public interface ICandidateService
{
    Task<bool> ExistAsync(int id);
    Task<Candidate?> GetAsync(int id);
    Task<XmlDocument> GetXmlAsync(int id);
    Task<OperationResult<int>> CreateAsync(Candidate item);
    Task<OperationResult> UpdateAsync(Candidate item);
    Task<OperationResult> DeleteAsync(int id);
    Task<IEnumerable<Candidate>> SearchAsync(CandidateSearch searchParams);
    Task<IEnumerable<Candidate>> GetRecentAsync(int count);
    Task<IEnumerable<CandidateOnVacancy>> GetCandidateOnVacanciesAsync(int candidateId);
    Task<IEnumerable<Comment>> GetCommentsAsync(int candidateId);
}
