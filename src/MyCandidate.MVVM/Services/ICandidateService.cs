using System.Collections.Generic;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public interface ICandidateService
{
    Candidate Get(int id);
    bool Create (Candidate item, out int id, out string message);
    bool Update (Candidate item, out string message);
    bool Delete (int id, out string message);
    IEnumerable<Candidate> Search(CandidateSearch searchParams);
    IEnumerable<Candidate> GetRecent(int count);
    IEnumerable<CandidateOnVacancy> GetCandidateOnVacancies(int candidateId);
    IEnumerable<Comment> GetComments(int candidateId);
}
