using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCandidate.Common;

public class CandidateOnVacancy
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Id", Order = 1)]
    public int Id { get; set; } 

    [ForeignKey(nameof(VacancyId))]
    [Display(Name = "Vacancy", Order = 2)]
    public int VacancyId { get; set; }

    [Browsable(false)]
    public Vacancy Vacancy { get; set; }   

    [ForeignKey(nameof(CandidateId))]
    [Display(Name = "Candidate", Order = 3)]
    public int CandidateId { get; set; }

    [Browsable(false)]
    public Candidate Candidate { get; set; } 

    [ForeignKey(nameof(SelectionStatusId))]
    [Display(Name = "Status", Order = 4)]
    public int SelectionStatusId { get; set; }

    [Browsable(false)]
    public SelectionStatus SelectionStatus { get; set; } 

    [Required]
    [Display(Name = "Creation Date", Order = 5)]
    public DateTime CreationDate { get; set; }    

    [Required]
    [Display(Name = "Last Modification Date", Order = 6)]
    public DateTime LastModificationDate { get; set; }  

    [Browsable(false)]
    public virtual List<Comment> Comments { get; set; }                         
}
