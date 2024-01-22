using System;
using System.Collections.Generic;
using log4net;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Services;

public class VacancyService : IVacancyService
{
    private readonly IDataAccess<Vacancy> _vacancies;
    private readonly ILog _log; 

    public VacancyService(IDataAccess<Vacancy> vacancies, ILog log)
    {
        _vacancies = vacancies;
        _log = log;
    }   
    
    public bool Create(Vacancy item, out string message)
    {
        message = string.Empty;
        try
        {
            _vacancies.Create(new List<Vacancy> { item });
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            _log.Error(ex);
            return false;
        }
    }

    public Vacancy Get(int id)
    {
        return _vacancies.Get(id);
    }

    public bool Update(Vacancy item, out string message)
    {
        message = string.Empty;
        try
        {
            _vacancies.Update(new List<Vacancy> { item });
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
