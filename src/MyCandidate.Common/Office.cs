using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class Office : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Browsable(false)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250, MinimumLength = 2)]
    [DisplayName("Office")]
    public override string Name { get; set; } = string.Empty;

    [ForeignKey(nameof(CompanyId))]
    [Browsable(false)]
    public int CompanyId { get; set; }

    [Required]
    public Company? Company { get; set; }

    [ForeignKey(nameof(LocationId))]
    [Browsable(false)]
    public int LocationId { get; set; }

    [Required]
    [DisplayName("Address")]
    public Location? Location { get; set; }

    [Required]
    [DefaultValue(true)]
    public override bool Enabled { get; set; }

    [Browsable(false)]
    public virtual List<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
