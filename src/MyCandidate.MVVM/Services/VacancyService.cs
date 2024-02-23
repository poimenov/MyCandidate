using System;
using System.Collections.Generic;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class VacancyService : IVacancyService
{
    private readonly IVacancies _vacancies;
    private readonly ICandidateOnVacancies _candidateOnVacancies;
    private readonly IComments _comments;
    private readonly ILog _log;

    public VacancyService(IVacancies vacancies, ICandidateOnVacancies candidateOnVacancies, IComments comments, ILog log)
    {
        _vacancies = vacancies;
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

    public Vacancy Get(int id)
    {
        return _vacancies.Get(id);
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
        //Add logic for sort by count skill found desc
        return _vacancies.Search(searchParams);
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
