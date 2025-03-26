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
    [Browsable(false)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250, MinimumLength = 2)]
    [DisplayName("City")]
    public override string Name { get; set; } = string.Empty;

    [ForeignKey(nameof(CountryId))]
    [Browsable(false)]
    public int CountryId { get; set; }

    [Required]
    public Country? Country { get; set; }

    [Required]
    [DefaultValue(true)]
    public override bool Enabled { get; set; }

    [Browsable(false)]
    public virtual List<Location> Locations { get; set; } = new List<Location>();
}
