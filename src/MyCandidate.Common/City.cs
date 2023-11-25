using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class City : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Id", Order = 1/*, ResourceType = typeof(Properties.Resource)*/)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250)]
    [Display(Name = "Название города", Order = 4/*, ResourceType = typeof(Properties.Resource)*/)]
    public override string Name { get; set; }

    [Required]
    [DefaultValue(true)]
    [Display(Name = "Enabled", Order = 2/*, ResourceType = typeof(Properties.Resource)*/)]
    public override bool Enabled { get; set; }

    [ForeignKey(nameof(CountryId))]
    [Browsable(false)]
    //[Display(Name = "Country", Order = 3/*, ResourceType = typeof(Properties.Resource)*/)]
    public int CountryId { get; set; }

    //[Browsable(false)]
    [Required]
    [Display(Name = "Country", Order = 3/*, ResourceType = typeof(Properties.Resource)*/)]
    public Country Country { get; set; }

    [Browsable(false)]
    public virtual List<Location> Locations { get; set; }    
}
