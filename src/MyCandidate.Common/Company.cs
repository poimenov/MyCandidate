using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common
{
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

    public class CompanyEqualityComparer : IEqualityComparer<Company>
    {
        public bool Equals(Company? x, Company? y)
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

        public int GetHashCode([DisallowNull] Company obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    }    
}

