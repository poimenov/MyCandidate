using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class Vacancy : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Id", Order = 1)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250)]
    [Display(Name = "Name", Order = 3)]
    public override string Name { get; set; }

    [Required]
    [DefaultValue(true)]
    [Display(Name = "Enabled", Order = 2)]
    public override bool Enabled { get; set; }

    [Display(Name = "Description", Order = 4)]
    public string Description { get; set; }    

    [ForeignKey(nameof(OfficeId))]
    [Display(Name = "Office", Order = 5)]
    public int OfficeId { get; set; }

    [Browsable(false)]
    public Office Office { get; set; }     

    [ForeignKey(nameof(VacancyStatusId))]
    [Display(Name = "Status", Order = 6)]
    public int VacancyStatusId { get; set; }

    [Browsable(false)]
    public VacancyStatus VacancyStatus { get; set; }  

    [Required]
    [Display(Name = "Creation Date", Order = 7)]
    public DateTime CreationDate { get; set; }    

    [Required]
    [Display(Name = "Last Modification Date", Order = 8)]
    public DateTime LastModificationDate { get; set; }   

    [Browsable(false)]
    public virtual List<VacancySkill> VacancySkills { get; set; } 

    [Browsable(false)]
    public virtual List<VacancyResource> VacancyResources { get; set; }    

    [Browsable(false)]
    public virtual List<CandidateOnVacancy> CandidateOnVacancies { get; set; }                 
}
