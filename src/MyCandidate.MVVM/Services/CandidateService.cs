using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Extensions;

namespace MyCandidate.MVVM.Services;

public class CandidateService : ICandidateService
{
    private readonly ICandidates _candidates;
    private readonly IVacancies _vacancies;
    private readonly ICandidateOnVacancies _candidateOnVacancies;
    private readonly IComments _comments;
    private readonly ILog _log;
    public CandidateService(ICandidates candidates, IVacancies vacancies, ICandidateOnVacancies candidateOnVacancies, IComments comments, ILog log)
    {
        _candidates = candidates;
        _vacancies = vacancies;
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

    public XmlDocument GetXml(int id)
    {
        var candidate = _candidates.Get(id);
        var root = candidate.ToXml();
        var candidateOnVacancies = new XElement("CandidateOnVacancies");
        foreach(var item in  candidate.CandidateOnVacancies)
        {
            var candidateOnVacancy = new XElement("CandidateOnVacancy", new XAttribute("status", item.SelectionStatus.Name),
                                                                        new XAttribute("created", item.CreationDate),
                                                                        new XAttribute("modified", item.LastModificationDate));
            candidateOnVacancy.Add(_vacancies.Get(item.VacancyId).ToXml());            
            candidateOnVacancies.Add(candidateOnVacancy);
        }  
        root.Add(candidateOnVacancies);      

        StringBuilder sb = new StringBuilder();
        using (var writer = XmlWriter.Create(sb))
        {
            root.Save(writer);
        }

        var retVal = new XmlDocument();
        retVal.LoadXml(sb.ToString());
        return retVal; 
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
