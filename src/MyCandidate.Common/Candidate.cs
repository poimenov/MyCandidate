using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common
{
    public class Candidate : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Browsable(false)]
        public override int Id { get; set; }

        [NotMapped]
        [DisplayName("Name")]
        public override string Name => string.Format("{0} {1}", this.LastName, this.FirstName);

        [Required]
        [StringLength(250, MinimumLength = 2)]
        [DisplayName("FirstName")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 2)]
        [DisplayName("LastName")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("BirthDate")]
        public DateTime BirthDate { get; set; }

        [ForeignKey(nameof(LocationId))]
        [Browsable(false)]
        public int LocationId { get; set; }

        [DisplayName("Address")]
        public Location Location { get; set; }

        [Required]
        [DisplayName("CreationDate")]
        public DateTime CreationDate { get; set; }

        [DisplayName("LastModificationDate")]
        public DateTime LastModificationDate { get; set; }

        [Required]
        [DefaultValue(true)]
        public override bool Enabled { get; set; }

        [Browsable(false)]
        public virtual List<CandidateSkill> CandidateSkills { get; set; }

        [Browsable(false)]
        public virtual List<CandidateResource> CandidateResources { get; set; }

        [Browsable(false)]
        public virtual List<CandidateOnVacancy> CandidateOnVacancies { get; set; }
    }

    public class CandidateEqualityComparer : IEqualityComparer<Candidate>
    {
        public bool Equals(Candidate? x, Candidate? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
                return false;

            return x.Id == y.Id
                && x.FirstName == y.FirstName
                && x.LastName == y.LastName
                && x.BirthDate == y.BirthDate
                && x.Enabled == y.Enabled;
        }

        public int GetHashCode([DisallowNull] Candidate obj)
        {
            return HashCode.Combine(obj.Id.GetHashCode(), 
                obj.FirstName.GetHashCode(), 
                obj.LastName.GetHashCode(),
                obj.BirthDate.GetHashCode(),
                obj.Enabled.GetHashCode());
        }
    }     
}
