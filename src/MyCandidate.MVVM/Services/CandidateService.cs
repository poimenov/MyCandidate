using System;
using System.Collections.Generic;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class CandidateService : ICandidateService
{
    private readonly IDataAccess<Candidate> _candidates;
    private readonly ILog _log;
    public CandidateService(IDataAccess<Candidate> candidates, ILog log)
    {
        _candidates = candidates;
        _log = log;
    }
    public bool Create(Candidate item, out string message)
    {
        message = string.Empty;
        try
        {
            _candidates.Create(new List<Candidate> { item });
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }

    public Candidate Get(int id)
    {
        return _candidates.Get(id);
    }

    public bool Update(Candidate item, out string message)
    {
        message = string.Empty;
        try
        {
            _candidates.Update(new List<Candidate> { item });
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }
}
