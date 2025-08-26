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

    public async Task<OperationResult<int>> CreateAsync(Candidate item)
    {
        var result = new OperationResult<int> { Success = false, Result = 0 };
        var createResult = await _candidates.CreateAsync(item);
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
            result.Message = "Failed to create candidate";
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
        var deleteResult = await _candidates.DeleteAsync(id);
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
            result.Message = "Failed to delete candidate";
        }
        else
        {
            result.Message = deleteResult.Message;
        }

        return result;
    }

    public Task<bool> ExistAsync(int id)
    {
        return _candidates.ExistAsync(id);
    }

    public Task<Candidate?> GetAsync(int id)
    {
        return _candidates.GetAsync(id);
    }

    public async Task<XmlDocument> GetXmlAsync(int id)
    {
        try
        {
            var candidate = await _candidates.GetAsync(id);
            if (candidate == null)
            {
                return new XmlDocument();
            }

            var root = candidate.ToXml();
            var candidateOnVacancies = new XElement("CandidateOnVacancies");
            foreach (var item in candidate.CandidateOnVacancies)
            {
                if (item.SelectionStatus != null && !string.IsNullOrEmpty(item.SelectionStatus.Name))
                {
                    var candidateOnVacancy = new XElement("CandidateOnVacancy", new XAttribute("status", item.SelectionStatus.Name),
                                                                        new XAttribute("created", item.CreationDate),
                                                                        new XAttribute("modified", item.LastModificationDate));
                    var vacancy = await _vacancies.GetAsync(item.VacancyId);
                    if (vacancy != null)
                    {
                        candidateOnVacancy.Add(vacancy.ToXml());
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
        catch (Exception ex)
        {
            _log.Error(ex);
            if (ex.InnerException != null)
            {
                _log.Error(ex.InnerException);
            }
            return new XmlDocument();
        }
    }

    public async Task<IEnumerable<CandidateOnVacancy>> GetCandidateOnVacanciesAsync(int candidateId)
    {
        return await _candidateOnVacancies.GetListByCandidateIdAsync(candidateId);
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(int candidateId)
    {
        return await _comments.GetCommentsByCandidateIdAsync(candidateId);
    }

    public async Task<IEnumerable<Candidate>> SearchAsync(CandidateSearch searchParams)
    {
        return await _candidates.SearchAsync(searchParams);
    }

    public async Task<IEnumerable<Candidate>> GetRecentAsync(int count)
    {
        return await _candidates.GetRecentAsync(count);
    }

    public async Task<OperationResult> UpdateAsync(Candidate item)
    {
        var result = new OperationResult { Success = false };
        var updateResult = await _candidates.UpdateAsync(item);
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
            result.Message = "Failed to update candidate";
        }
        else
        {
            result.Message = updateResult.Message;
        }

        return result;
    }
}
