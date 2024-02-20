using System.Data;

namespace MyCandidate.Common.Interfaces;

public interface ICandidateOnVacancies
{
    public IEnumerable<CandidateOnVacancy> GetListByCandidateId(int candidateId);
    public IEnumerable<CandidateOnVacancy> GetListByVacancyId(int vacancyId);
    void Update(IEnumerable<CandidateOnVacancy> candidateOnVacancies);
    void DeleteByCandidateId(int candidateId);
    void DeleteByVacancyId(int vacancyId);
}
