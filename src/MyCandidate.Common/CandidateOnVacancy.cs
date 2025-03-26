using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyModels.ComponentModel;

namespace MyCandidate.Common;

public class CandidateOnVacancy : ReactiveObject
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Browsable(false)]
    public int Id { get; set; }

    [ForeignKey(nameof(VacancyId))]
    [Browsable(false)]
    public int VacancyId { get; set; }

    [Browsable(false)]
    public Vacancy? Vacancy { get; set; }

    [ForeignKey(nameof(CandidateId))]
    [Browsable(false)]
    public int CandidateId { get; set; }

    [Browsable(false)]
    public Candidate? Candidate { get; set; }

    [ForeignKey(nameof(SelectionStatusId))]
    [Browsable(false)]
    public int SelectionStatusId { get; set; }

    public virtual SelectionStatus? SelectionStatus { get; set; }

    [Required]
    [Browsable(false)]
    public DateTime CreationDate { get; set; }

    [Required]
    [Browsable(false)]
    public DateTime LastModificationDate { get; set; }

    [Browsable(false)]
    public virtual List<Comment> Comments { get; set; } = new List<Comment>();
}
