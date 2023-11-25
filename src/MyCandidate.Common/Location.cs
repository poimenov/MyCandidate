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
    [Display(Name = "Id", Order = 1/*, ResourceType = typeof(Properties.Resource)*/)]
    public override int Id { get; set; }

    [Required]
    [StringLength(250)]
    [Display(Name = "Address", Order = 3/*, ResourceType = typeof(Properties.Resource)*/)]
    public string Address { get; set; }

    [Required]
    [DefaultValue(true)]
    [Display(Name = "Enabled", Order = 4/*, ResourceType = typeof(Properties.Resource)*/)]
    public override bool Enabled { get; set; }

    [NotMapped]
    [Display(Name = "FullAddress", Order = 5/*, ResourceType = typeof(Properties.Resource)*/)]
    public override string Name
    {
        get
        {
            return string.Format("{0}, {1}, {2}", City.Country?.Name, City.Name, Address);
        }
    }

    [ForeignKey(nameof(CityId))]
    [Display(Name = "City", Order = 2/*, ResourceType = typeof(Properties.Resource)*/)]
    public int CityId { get; set; }

    [Browsable(false)]
    public City City { get; set; }

    [Browsable(false)]
    public virtual List<Office> Officies { get; set; }    

    [Browsable(false)]
    public virtual List<Candidate> Candidates { get; set; }      
}