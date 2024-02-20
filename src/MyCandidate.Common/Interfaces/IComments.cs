namespace MyCandidate.Common.Interfaces;

public interface IComments
{    
    IEnumerable<Comment> GetComments(int candidateOnVacancyId);
    void Update(IEnumerable<Comment> comments);
    void DeleteByCandidateOnVacancyId(int candidateOnVacancyId);
}
