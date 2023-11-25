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

    [ForeignKey(nameof(CompanyId))]
    [Display(Name = "Company", Order = 3)]
    public int CompanyId { get; set; }

    [Browsable(false)]
    public Company Company { get; set; }

    [ForeignKey(nameof(LocationId))]
    [Display(Name = "Location", Order = 3)]
    public int LocationId { get; set; }

    [Browsable(false)]
    public Location Location { get; set; }    

    [Browsable(false)]
    public virtual List<Vacancy> Vacancies { get; set; }    
}
