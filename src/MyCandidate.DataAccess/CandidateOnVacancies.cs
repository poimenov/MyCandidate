using Microsoft.EntityFrameworkCore;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class CandidateOnVacancies : ICandidateOnVacancies
{
    private readonly IComments _comments;
    public CandidateOnVacancies(IComments comments)
    {
        _comments = comments;
    }

    public void DeleteByCandidateId(int candidateId)
    {
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var items = db.CandidateOnVacancies.Where(x => x.CandidateId == candidateId).ToList();
                    items.ForEach(x => _comments.DeleteByCandidateOnVacancyId(x.Id));
                    db.CandidateOnVacancies.RemoveRange(items);
                    
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        } 
    }

    public void DeleteByVacancyId(int vacancyId)
    {
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var items = db.CandidateOnVacancies.Where(x => x.VacancyId == vacancyId).ToList();
                    items.ForEach(x => _comments.DeleteByCandidateOnVacancyId(x.Id));
                    db.CandidateOnVacancies.RemoveRange(items);
                    
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        } 
    }

    public IEnumerable<CandidateOnVacancy> GetListByCandidateId(int candidateId)
    {
        using (var db = new Database())
        {
            return db.CandidateOnVacancies
                        .Where(x => x.CandidateId == candidateId)
                        .Include(x => x.Candidate)
                        .Include(x => x.Vacancy)
                        .Include(x => x.SelectionStatus)
                        .ToList();
        }
    }

    public IEnumerable<CandidateOnVacancy> GetListByVacancyId(int vacancyId)
    {
        using (var db = new Database())
        {
            return db.CandidateOnVacancies
                        .Where(x => x.VacancyId == vacancyId)
                        .Include(x => x.Candidate)
                        .Include(x => x.Vacancy)
                        .Include(x => x.SelectionStatus)
                        .ToList();
        }
    }

    public void Update(IEnumerable<CandidateOnVacancy> candidateOnVacancies)
    {
        if(candidateOnVacancies == null || !candidateOnVacancies.Any())
        {
            return;
        }

        var dateTime = DateTime.Now;
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //update existed candidateOnVacancies
                    var idsToUpdate = candidateOnVacancies.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
                    foreach (var idToUpdate in idsToUpdate)
                    {
                        if (db.CandidateOnVacancies.Any(x => x.Id == idToUpdate))
                        {
                            var candidateOnVacancyToUpdate = candidateOnVacancies.First(x => x.Id == idToUpdate);
                            var candidateOnVacancy = db.CandidateOnVacancies.First(x => x.Id == idToUpdate);
                            candidateOnVacancy.SelectionStatusId = candidateOnVacancyToUpdate.SelectionStatusId;
                            candidateOnVacancy.LastModificationDate = dateTime;
                            _comments.Update(candidateOnVacancyToUpdate.Comments);
                        }
                    }
                    //delete existed candidateOnVacancies
                    var idsToDelete = db.CandidateOnVacancies.Where(x => !idsToUpdate.Contains(x.Id)).Select(x => x.Id).ToArray();
                    foreach (var idToDelete in idsToDelete)
                    {
                        if (db.CandidateOnVacancies.Any(x => x.Id == idToDelete))
                        {
                            db.Comments.RemoveRange(db.Comments.Where(x => x.CandidateOnVacancyId == idToDelete).ToList());
                            db.CandidateOnVacancies.Remove(db.CandidateOnVacancies.First(x => x.Id == idToDelete));
                        }
                    }
                    //add new candidateOnVacancies
                    foreach (var candidateOnVacancyToAdd in candidateOnVacancies.Where(x => x.Id == 0))
                    {
                        var newCandidateOnVacancy = new CandidateOnVacancy
                        {
                            CandidateId = candidateOnVacancyToAdd.CandidateId,
                            VacancyId = candidateOnVacancyToAdd.VacancyId,
                            SelectionStatusId = candidateOnVacancyToAdd.SelectionStatusId,
                            CreationDate = dateTime,
                            LastModificationDate = dateTime
                        };
                        db.CandidateOnVacancies.Add(newCandidateOnVacancy);
                        candidateOnVacancyToAdd.Comments.ForEach(x => x.CandidateOnVacancyId = newCandidateOnVacancy.Id);
                        _comments.Update(candidateOnVacancyToAdd.Comments);
                    }
                    
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
