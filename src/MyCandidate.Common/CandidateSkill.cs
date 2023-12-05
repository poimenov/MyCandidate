using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common;

namespace MyCandidate.Common;

public class CandidateSkill
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }  

    [ForeignKey(nameof(CandidateId))]
    public int CandidateId { get; set; }  
    
    [Browsable(false)]
    public Candidate Candidate { get; set; }      

    [ForeignKey(nameof(SkillId))]
    public int SkillId { get; set; }  
    
    [Browsable(false)]
    public Skill Skill { get; set; }    

    [ForeignKey(nameof(SeniorityId))]
    public int SeniorityId { get; set; }  
    
    [Browsable(false)]
    public Seniority Seniority { get; set; }   
}
