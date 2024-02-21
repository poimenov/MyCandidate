using System.ComponentModel;
using MyCandidate.Common;

namespace MyCandidate.MVVM.Models;

public class CandidateOnVacancyExt : CandidateOnVacancy
{
    public CandidateOnVacancyExt(CandidateOnVacancy candidateOnVacancy)
    {
        Id = candidateOnVacancy.Id;
        Candidate = candidateOnVacancy.Candidate;
        CandidateId = candidateOnVacancy.CandidateId;
        Vacancy = candidateOnVacancy.Vacancy;
        VacancyId = candidateOnVacancy.VacancyId;
        SelectionStatus = candidateOnVacancy.SelectionStatus;
        SelectionStatusId = candidateOnVacancy.SelectionStatusId;
        CreationDate = candidateOnVacancy.CreationDate;
        LastModificationDate = candidateOnVacancy.LastModificationDate;
        Comments = candidateOnVacancy.Comments;
    }

    [DisplayName("Vacancy")]
    [Category("Main")]
    public string VacancyName => Vacancy.Name;

    [DisplayName("Candidate")]
    [Category("Main")]
    public string CandidateName => Candidate.Name;

    [DisplayName("Selection_Status")]
    [Category("Main")]
    public override SelectionStatus SelectionStatus { get; set; } 

    [DisplayName("Creation_Date")]        
    public string Created => CreationDate.ToString("G");

    [DisplayName("Last_Modification_Date")]
    public string Updated => LastModificationDate.ToString("G");

    public CandidateOnVacancy TpCandidateOnVacancy()
    {
        return new CandidateOnVacancy
        {
            Id = this.Id,
            Candidate = this.Candidate,
            CandidateId = this.CandidateId,
            Vacancy = this.Vacancy,
            VacancyId = this.VacancyId,
            SelectionStatus = this.SelectionStatus,
            SelectionStatusId = this.SelectionStatusId,
            CreationDate = this.CreationDate,
            LastModificationDate = this.LastModificationDate,
            Comments = this.Comments
        };
    }
}
