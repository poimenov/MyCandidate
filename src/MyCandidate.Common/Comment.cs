using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyModels.ComponentModel;

namespace MyCandidate.Common;

public class Comment : ReactiveObject
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Browsable(false)]
    public int Id { get; set; }

    [Required]
    [StringLength(2000)]
    [DisplayName("Comment")]
    public virtual string Value { get; set; }   

    [ForeignKey(nameof(CandidateOnVacancyId))]
    [Browsable(false)]
    public int CandidateOnVacancyId { get; set; }  
    
    [Browsable(false)]
    public CandidateOnVacancy CandidateOnVacancy { get; set; } 

    [Required]
    [Browsable(false)]
    public DateTime CreationDate { get; set; }    

    [Required]
    [Browsable(false)]
    public DateTime LastModificationDate { get; set; }          
}
