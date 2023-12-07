using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common;

public class Candidate : Entity
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "Id", Order = 1)]
    public override int Id { get; set; }

    [NotMapped]
    [Display(Name = "Name", Order = 5)]
    public override string Name => string.Format("{0}: {1}", this.LastName, this.FirstName);    

    [Required]
    [DefaultValue(true)]
    [Display(Name = "Enabled", Order = 2)]
    public override bool Enabled { get; set; }

    [Required]
    [StringLength(250)]
    [Display(Name = "Name", Order = 4)]
    public string FirstName { get; set; }  

    [Required]
    [StringLength(250)]
    [Display(Name = "Name", Order = 3)]
    public string LastName { get; set; }   

    [ForeignKey(nameof(LocationId))]
    [Display(Name = "Location", Order = 6)]
    public int LocationId { get; set; }

    [Browsable(false)]
    public Location Location { get; set; }   

    [Required]
    [Display(Name = "BirthDate", Order = 7)]
    public DateTime BirthDate { get; set; }         

    [Required]
    [Display(Name = "Creation Date", Order = 8)]
    public DateTime CreationDate { get; set; }    

    [Required]
    [Display(Name = "Last Modification Date", Order = 9)]
    public DateTime LastModificationDate { get; set; }   

    [Browsable(false)]
    public virtual List<CandidateSkill> CandidateSkills { get; set; }     

    [Browsable(false)]
    public virtual List<CandidateResource> CandidateResources { get; set; }  

    [Browsable(false)]
    public virtual List<CandidateOnVacancy> CandidateOnVacancies { get; set; }      
}
