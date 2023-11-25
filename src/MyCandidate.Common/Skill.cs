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
    [Display(Name = "Id", Order = 1)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250)]
    [Display(Name = "Name", Order = 4)]
    public override string Name { get; set; }

    [Required]
    [DefaultValue(true)]
    [Display(Name = "Enabled", Order = 2)]
    public override bool Enabled { get; set; }

    [ForeignKey(nameof(SkillCategoryId))]
    [Display(Name = "Skill Category", Order = 3)]
    public int SkillCategoryId { get; set; }

    [Browsable(false)]
    public SkillCategory SkillCategory { get; set; }

    [Browsable(false)]
    public virtual List<VacancySkill> VacancySkills { get; set; }    
}
