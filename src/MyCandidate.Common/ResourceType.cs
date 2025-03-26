using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common
{
    public class ResourceType : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id", Order = 1)]
        public override int Id { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Name", Order = 3)]
        public override string Name { get; set; } = string.Empty;

        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2)]
        public override bool Enabled { get; set; }

        [Browsable(false)]
        public virtual List<CandidateResource> CandidateResources { get; set; } = new List<CandidateResource>();

        [Browsable(false)]
        public virtual List<VacancyResource> VacancyResources { get; set; } = new List<VacancyResource>();
    }

    public class ResourceTypeEqualityComparer : IEqualityComparer<ResourceType?>
    {
        public bool Equals(ResourceType? x, ResourceType? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
                return false;

            return x.Id == y.Id
                && x.Name == y.Name
                && x.Enabled == y.Enabled;
        }

        public int GetHashCode([DisallowNull] ResourceType obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    }
}