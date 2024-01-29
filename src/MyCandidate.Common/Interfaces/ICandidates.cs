namespace MyCandidate.Common.Interfaces;

public interface ICandidates
{
    bool Exist(string lastName, string firstName, DateTime birthdate);
    Candidate Get(int id);
    void Delete(int id);
    bool Create(Candidate candidate, out int id);
    void Update(Candidate candidate);
}
