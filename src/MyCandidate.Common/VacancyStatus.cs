using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class VacancyStatus : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Id", Order = 1)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250)]
    [Display(Name = "Name", Order = 3)]
    public override string Name { get; set; } = string.Empty;

    [Required]
    [DefaultValue(true)]
    [Display(Name = "Enabled", Order = 2)]
    public override bool Enabled { get; set; }

    [Browsable(false)]
    public virtual List<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
