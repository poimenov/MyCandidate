using MyCandidate.Common;

namespace MyCandidate.MVVM.Services;

public interface ICandidateService
{
    Candidate Get(int id);
    bool Create (Candidate item, out string message);
    bool Update (Candidate item, out string message);
}
