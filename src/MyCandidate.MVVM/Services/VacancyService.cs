using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task<OperationResult<int>> CreateAsync(Vacancy item)
    {
        var result = new OperationResult<int> { Success = false, Result = 0 };
        var createResult = await _vacancies.CreateAsync(item);
        if (createResult.Success)
        {
            result.Success = true;
            result.Result = createResult.Result;
        }
        else if (createResult.Exception != null)
        {
            _log.Error(createResult.Exception);
            if (createResult.Exception.InnerException != null)
            {
                _log.Error(createResult.Exception.InnerException);
            }
            result.Message = "Failed to create vacancy";
        }
        else
        {
            result.Message = createResult.Message;
        }

        return result;
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var result = new OperationResult { Success = false };
        var deleteResult = await _vacancies.DeleteAsync(id);
        if (deleteResult.Success)
        {
            result.Success = true;
        }
        else if (deleteResult.Exception != null)
        {
            _log.Error(deleteResult.Exception);
            if (deleteResult.Exception.InnerException != null)
            {
                _log.Error(deleteResult.Exception.InnerException);
            }
            result.Message = "Failed to delete vacancy";
        }
        else
        {
            result.Message = deleteResult.Message;
        }

        return result;
    }

    public Task<bool> ExistAsync(int id)
    {
        return _vacancies.ExistAsync(id);
    }

    public Task<Vacancy?> GetAsync(int id)
    {
        return _vacancies.GetAsync(id);
    }

    public async Task<XmlDocument> GetXmlAsync(int id)
    {
        var vacancy = await _vacancies.GetAsync(id);
        if (vacancy == null)
        {
            return new XmlDocument();
        }
        var root = vacancy.ToXml();
        var candidateOnVacancies = new XElement("CandidateOnVacancies");
        foreach (var item in vacancy.CandidateOnVacancies)
        {
            if (item.SelectionStatus != null && !string.IsNullOrEmpty(item.SelectionStatus.Name))
            {
                var candidateOnVacancy = new XElement("CandidateOnVacancy", new XAttribute("status", item.SelectionStatus.Name),
                                                                            new XAttribute("created", item.CreationDate),
                                                                            new XAttribute("modified", item.LastModificationDate));
                var candidate = await _candidates.GetAsync(item.CandidateId);
                if (candidate != null)
                {
                    candidateOnVacancy.Add(candidate.ToXml());
                    candidateOnVacancies.Add(candidateOnVacancy);
                }
            }
        }
        root.Add(candidateOnVacancies);

        StringBuilder sb = new StringBuilder();
        var settings = new XmlWriterSettings { Async = true };
        await using (var writer = XmlWriter.Create(sb, settings))
        {
            await root.SaveAsync(writer, CancellationToken.None);
        }

        var retVal = new XmlDocument();
        retVal.LoadXml(sb.ToString());
        return retVal;
    }

    public async Task<IEnumerable<CandidateOnVacancy>> GetCandidateOnVacanciesAsync(int vacancyId)
    {
        return await _candidateOnVacancies.GetListByVacancyIdAsync(vacancyId);
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(int vacancyId)
    {
        return await _comments.GetCommentsByVacancyIdAsync(vacancyId);
    }

    public async Task<IEnumerable<Vacancy>> SearchAsync(VacancySearch searchParams)
    {
        return await _vacancies.SearchAsync(searchParams);
    }

    public async Task<IEnumerable<Vacancy>> GetRecentasync(int count)
    {
        return await _vacancies.GetRecentAsync(count);
    }

    public async Task<OperationResult> UpdateAsync(Vacancy item)
    {
        var result = new OperationResult { Success = false };
        var updateResult = await _vacancies.UpdateAsync(item);
        if (updateResult.Success)
        {
            result.Success = true;
        }
        else if (updateResult.Exception != null)
        {
            _log.Error(updateResult.Exception);
            if (updateResult.Exception.InnerException != null)
            {
                _log.Error(updateResult.Exception.InnerException);
            }
            result.Message = "Failed to update vacancy";
        }
        else
        {
            result.Message = updateResult.Message;
        }

        return result;
    }
}
