using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common;
using PropertyModels.ComponentModel;

namespace MyCandidate.Common;

public class CandidateSkill : ReactiveObject
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Browsable(false)]
    public int Id { get; set; }  

    [ForeignKey(nameof(CandidateId))]
    [Browsable(false)]
    public int CandidateId { get; set; }  
    
    [Browsable(false)]
    public Candidate Candidate { get; set; }      

    [ForeignKey(nameof(SkillId))]
    [Browsable(false)]
    public int SkillId { get; set; }  
    
    [Required]
    [DisplayName("Skill")]       
    public Skill Skill { get; set; }    

    [ForeignKey(nameof(SeniorityId))]
    [Browsable(false)]
    public int SeniorityId { get; set; }  
    
    [Required]
    [DisplayName("Seniority")]    
    public Seniority Seniority { get; set; }   
}
