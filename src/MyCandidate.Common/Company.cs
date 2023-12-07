using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class Company : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Browsable(false)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250, MinimumLength = 2)]
    [DisplayName("Company")]
    public override string Name { get; set; }

    [Required]
    [DefaultValue(true)]
    public override bool Enabled { get; set; }

    [Browsable(false)]
    public virtual List<Office> Officies { get; set; }
}
