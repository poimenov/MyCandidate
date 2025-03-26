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
    [Browsable(false)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250, MinimumLength = 3)]
    [DisplayName("Name")]
    public override string Name { get; set; } = string.Empty;

    [Required]
    [DefaultValue(true)]
    [DisplayName("Enabled")]
    public override bool Enabled { get; set; }

    [DisplayName("Description")]
    public string Description { get; set; } = string.Empty;

    [ForeignKey(nameof(OfficeId))]
    [Browsable(false)]
    public int OfficeId { get; set; }

    [Required]
    [DisplayName("Office")]
    public Office? Office { get; set; }

    [ForeignKey(nameof(VacancyStatusId))]
    [Browsable(false)]
    public int VacancyStatusId { get; set; }

    [Required]
    [DisplayName("Vacancy_Status")]
    public VacancyStatus? VacancyStatus { get; set; }

    [Required]
    [DisplayName("CreationDate")]
    public DateTime CreationDate { get; set; }

    [DisplayName("LastModificationDate")]
    public DateTime LastModificationDate { get; set; }

    [Browsable(false)]
    public virtual List<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();

    [Browsable(false)]
    public virtual List<VacancyResource> VacancyResources { get; set; } = new List<VacancyResource>();

    [Browsable(false)]
    public virtual List<CandidateOnVacancy> CandidateOnVacancies { get; set; } = new List<CandidateOnVacancy>();
}
