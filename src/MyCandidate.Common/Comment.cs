using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCandidate.Common;

public class Comment
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Id", Order = 1)]
    public int Id { get; set; }

    [Required]
    [StringLength(2000)]
    [Display(Name = "Value", Order = 2)]
    public string Value { get; set; }   

    [ForeignKey(nameof(CandidateOnVacancyId))]
    public int CandidateOnVacancyId { get; set; }  
    
    [Browsable(false)]
    public CandidateOnVacancy CandidateOnVacancy { get; set; } 

    [Required]
    [Display(Name = "Creation Date", Order = 3)]
    public DateTime CreationDate { get; set; }    

    [Required]
    [Display(Name = "Last Modification Date", Order = 4)]
    public DateTime LastModificationDate { get; set; }          
}
