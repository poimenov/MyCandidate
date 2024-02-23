using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyCandidate.Common;
using PropertyModels.ComponentModel;

namespace MyCandidate.MVVM.Models;

public class CommentExt : Comment
{
    public CommentExt()
    {
        //
    }
    public CommentExt(Comment comment)
    {
        Id = comment.Id;
        Value = comment.Value;
        CandidateOnVacancy = comment.CandidateOnVacancy;
        CandidateOnVacancyId = comment.CandidateOnVacancyId;
        CreationDate = comment.CreationDate;
        LastModificationDate = comment.LastModificationDate;
    }
    [DisplayName("Vacancy")]
    [Category("Main")]
    public string VacancyName => CandidateOnVacancy.Vacancy.Name;

    [DisplayName("Candidate")]
    [Category("Main")]
    public string CandidateName => CandidateOnVacancy.Candidate.Name;

    [Required]
    [StringLength(2000)]
    [MultilineText]
    [DisplayName("Comment")]
    [Category("Main")]
    public override string Value { get; set; }      
    
    [DisplayName("Creation_Date")]
    public string Created => CreationDate.ToString("G");

    [DisplayName("Last_Modification_Date")]
    public string Updated => LastModificationDate.ToString("G");

    public Comment ToComment()
    {
        return new Comment
        {
            Id = this.Id,
            Value = this.Value,
            CandidateOnVacancy = this.CandidateOnVacancy,
            CandidateOnVacancyId = this.CandidateOnVacancyId,
            CreationDate = this.CreationDate,
            LastModificationDate = this.LastModificationDate
        };
    }

    public bool IsValid
    {
        get
        {
            return Validator.TryValidateObject(this, new ValidationContext(this), null, true);
        }
    }
}
