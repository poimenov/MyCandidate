using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class Comments : IComments
{
    public void DeleteByCandidateOnVacancyId(int candidateOnVacancyId)
    {
        using (var db = new Database())
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Comments.RemoveRange(db.Comments.Where(x => x.CandidateOnVacancyId == candidateOnVacancyId));
                    
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

    public IEnumerable<Comment> GetComments(int candidateOnVacancyId)
    {
        using (var db = new Database())
        {
            return db.Comments
                        .Where(x => x.CandidateOnVacancyId == candidateOnVacancyId)
                        .ToList();
        }
    }

    public void Update(IEnumerable<Comment> comments)
    {
        if(comments == null || !comments.Any())
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
                    //update existed comments
                    var idsToUpdate = comments.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
                    foreach (var idToUpdate in idsToUpdate)
                    {
                        if (db.Comments.Any(x => x.Id == idToUpdate))
                        {
                            var commentToUpdate = comments.First(x => x.Id == idToUpdate);
                            var comment = db.Comments.First(x => x.Id == idToUpdate);
                            comment.Value = commentToUpdate.Value;
                            comment.LastModificationDate = dateTime;
                        }
                    }
                    //delete existed comments
                    var idsToDelete = db.Comments.Where(x => !idsToUpdate.Contains(x.Id)).Select(x => x.Id).ToArray();
                    foreach (var idToDelete in idsToDelete)
                    {
                        if (db.Comments.Any(x => x.Id == idToDelete))
                        {
                            db.Comments.Remove(db.Comments.First(x => x.Id == idToDelete));
                        }
                    }
                    //add new comments
                    foreach (var commentToAdd in comments.Where(x => x.Id == 0))
                    {
                        var newComment = new Comment
                        {
                            CandidateOnVacancyId = commentToAdd.CandidateOnVacancyId,
                            Value = commentToAdd.Value,
                            CreationDate = dateTime,
                            LastModificationDate = dateTime
                        };
                        db.Comments.Add(newComment);
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
