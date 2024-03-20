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

public class VacancyService : IVacancyService
{
    private readonly IVacancies _vacancies;
    private readonly ICandidates _candidates;
    private readonly ICandidateOnVacancies _candidateOnVacancies;
    private readonly IComments _comments;
    private readonly ILog _log;

    public VacancyService(IVacancies vacancies, ICandidates candidates, ICandidateOnVacancies candidateOnVacancies, IComments comments, ILog log)
    {
        _vacancies = vacancies;
        _candidates = candidates;
        _candidateOnVacancies = candidateOnVacancies;
        _comments = comments;
        _log = log;
    }

    public bool Create(Vacancy item, out int id, out string message)
    {
        message = string.Empty;
        id = 0;
        bool retVal = false;
        try
        {
            _vacancies.Create(item, out id);
            retVal = true;
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
            _vacancies.Delete(id);
            retVal = true;
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

    public bool Exist(int id)
    {
        return _vacancies.Exist(id);
    }

    public Vacancy Get(int id)
    {
        return _vacancies.Get(id);
    }

    public XmlDocument GetXml(int id)
    {
        var vacancy = _vacancies.Get(id);
        var root = vacancy.ToXml();
        var candidateOnVacancies = new XElement("CandidateOnVacancies");
        foreach (var item in vacancy.CandidateOnVacancies)
        {
            var candidateOnVacancy = new XElement("CandidateOnVacancy", new XAttribute("status", item.SelectionStatus.Name),
                                                                        new XAttribute("created", item.CreationDate),
                                                                        new XAttribute("modified", item.LastModificationDate));
            candidateOnVacancy.Add(_candidates.Get(item.CandidateId).ToXml());
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

    public IEnumerable<CandidateOnVacancy> GetCandidateOnVacancies(int vacancyId)
    {
        return _candidateOnVacancies.GetListByVacancyId(vacancyId);
    }

    public IEnumerable<Comment> GetComments(int vacancyId)
    {
        return _comments.GetCommentsByVacancyId(vacancyId);
    }

    public IEnumerable<Vacancy> Search(VacancySearch searchParams)
    {
        return _vacancies.Search(searchParams);
    }

    public IEnumerable<Vacancy> GetRecent(int count)
    {
        return _vacancies.GetRecent(count);
    }

    public bool Update(Vacancy item, out string message)
    {
        message = string.Empty;
        bool retVal = false;

        try
        {
            _vacancies.Update(item);
            retVal = true;
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
}
