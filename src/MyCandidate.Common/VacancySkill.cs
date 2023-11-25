using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCandidate.Common;

public class VacancySkill
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }  

    [ForeignKey(nameof(VacancyId))]
    public int VacancyId { get; set; }  
    
    [Browsable(false)]
    public Vacancy Vacancy { get; set; }      

    [ForeignKey(nameof(SkillId))]
    public int SkillId { get; set; }  
    
    [Browsable(false)]
    public Skill Skill { get; set; }    

    [ForeignKey(nameof(SeniorityId))]
    public int SeniorityId { get; set; }  
    
    [Browsable(false)]
    public Seniority Seniority { get; set; }          
}
