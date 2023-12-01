using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class Skill : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Browsable(false)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250)]
    [DisplayName("Skill")]
    public override string Name { get; set; }

    [ForeignKey(nameof(SkillCategoryId))]
    [Browsable(false)]
    public int SkillCategoryId { get; set; }

    [Required]
    public SkillCategory SkillCategory { get; set; }

    [Required]
    [DefaultValue(true)]
    public override bool Enabled { get; set; }    

    [Browsable(false)]
    public virtual List<VacancySkill> VacancySkills { get; set; }    
}
