using System;
using System.Collections.Generic;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class CandidateService : ICandidateService
{
    private readonly ICandidates _candidates;
    private readonly ICandidateOnVacancies _candidateOnVacancies;
    private readonly IComments _comments;
    private readonly ILog _log;
    public CandidateService(ICandidates candidates, ICandidateOnVacancies candidateOnVacancies, IComments comments, ILog log)
    {
        _candidates = candidates;
        _candidateOnVacancies = candidateOnVacancies;
        _comments = comments;
        _log = log;
    }

    public bool Create(Candidate item, out int id, out string message)
    {
        message = string.Empty;
        id = 0;
        bool retVal = false;

        try
        {
            if (_candidates.Exist(item.LastName, item.FirstName, item.BirthDate))
            {
                message = "A candidate with the same last name, first name and birthday already exists";
            }
            else if (_candidates.Create(item, out id))
            {
                retVal = true;
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            if (ex.InnerException != null)
            {
                message = ex.InnerException.Message;
                _log.Error(ex.InnerException);
            }
        }

        return retVal;
    }

    public bool Delete(int id, out string message)
    {
        message = string.Empty;
        bool retVal = false;

        try
        {
            _candidates.Delete(id);
            retVal = true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
        }

        return retVal;
    }

    public bool Exist(int id)
    {
        return _candidates.Exist(id);
    }

    public Candidate Get(int id)
    {
        return _candidates.Get(id);
    }

    public IEnumerable<CandidateOnVacancy> GetCandidateOnVacancies(int candidateId)
    {
        return _candidateOnVacancies.GetListByCandidateId(candidateId);
    }

    public IEnumerable<Comment> GetComments(int candidateId)
    {
        return _comments.GetCommentsByCandidateId(candidateId);
    }

    public IEnumerable<Candidate> Search(CandidateSearch searchParams)
    {
        return _candidates.Search(searchParams);
    }

    public IEnumerable<Candidate> GetRecent(int count)
    {
        return _candidates.GetRecent(count);
    }

    public bool Update(Candidate item, out string message)
    {
        message = string.Empty;
        bool retVal = false;

        try
        {
            _candidates.Update(item);
            retVal = true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
        }

        return retVal;
    }
}
