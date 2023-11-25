using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCandidate.Common;

public class CandidateResource
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Id", Order = 1)]
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    [Display(Name = "Value", Order = 2)]
    public string Value { get; set; }   

    [ForeignKey(nameof(CandidateId))]
    public int CandidateId { get; set; }  
    
    [Browsable(false)]
    public Candidate Candidate { get; set; }   
    
    [ForeignKey(nameof(ResourceTypeId))]
    public int ResourceTypeId { get; set; }  
    
    [Browsable(false)]
    public ResourceType ResourceType { get; set; }           
}
