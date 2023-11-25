using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class Country : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Id", Order = 1/*, ResourceType = typeof(Properties.Resource)*/)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250, MinimumLength = 3)]
    [Display(Name = "Name", Order = 3/*, ResourceType = typeof(Properties.Resource)*/)]
    public override string Name { get; set; }

    [Required]
    [DefaultValue(true)]
    [Display(Name = "Enabled", Order = 2/*, ResourceType = typeof(Properties.Resource)*/)]
    public override bool Enabled { get; set; }

    [Browsable(false)]
    public virtual List<City> Cities { get; set; }
}
