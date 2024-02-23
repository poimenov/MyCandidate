namespace MyCandidate.Common.Interfaces;

public interface IComments
{    
    IEnumerable<Comment> GetCommentsByCandidateId(int candidateId);
    IEnumerable<Comment> GetCommentsByVacancyId(int vacancyId);
}
