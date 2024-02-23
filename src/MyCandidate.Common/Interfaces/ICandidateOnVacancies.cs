namespace MyCandidate.Common.Interfaces;

public interface ICandidateOnVacancies
{
    public IEnumerable<CandidateOnVacancy> GetListByCandidateId(int candidateId);
    public IEnumerable<CandidateOnVacancy> GetListByVacancyId(int vacancyId);
}
