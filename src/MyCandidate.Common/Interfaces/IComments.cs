namespace MyCandidate.Common.Interfaces;

public interface IComments
{
    Task<IEnumerable<Comment>> GetCommentsByCandidateIdAsync(int candidateId);
    Task<IEnumerable<Comment>> GetCommentsByVacancyIdAsync(int vacancyId);
}
