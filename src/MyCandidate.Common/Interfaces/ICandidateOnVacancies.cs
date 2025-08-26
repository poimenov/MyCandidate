namespace MyCandidate.Common.Interfaces;

public interface ICandidateOnVacancies
{
    Task<IEnumerable<CandidateOnVacancy>> GetListByCandidateIdAsync(int candidateId);
    Task<IEnumerable<CandidateOnVacancy>> GetListByVacancyIdAsync(int vacancyId);
}
