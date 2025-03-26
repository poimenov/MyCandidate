using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class Location : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Browsable(false)]
    public override int Id { get; set; }

    //[Required]
    [StringLength(250)]
    [DisplayName("Address")]
    public string Address { get; set; } = string.Empty;

    [NotMapped]
    [DisplayName("Full_Address")]
    public override string Name
    {
        get
        {
            return string.Format("{0}, {1}, {2}", City?.Country?.Name, City?.Name, Address);
        }
    }

    [ForeignKey(nameof(CityId))]
    [Browsable(false)]
    public int CityId { get; set; }

    [Required]
    public City? City { get; set; }

    [Required]
    [DefaultValue(true)]
    public override bool Enabled { get; set; }

    [Browsable(false)]
    public virtual List<Office> Officies { get; set; } = new List<Office>();

    [Browsable(false)]
    public virtual List<Candidate> Candidates { get; set; } = new List<Candidate>();
}