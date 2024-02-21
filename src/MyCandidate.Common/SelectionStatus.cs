using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common
{
    public class SelectionStatus : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id", Order = 1)]
        public override int Id { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Name", Order = 3)]
        public override string Name { get; set; }

        [Required]
        [DefaultValue(true)]
        [Display(Name = "Enabled", Order = 2)]
        public override bool Enabled { get; set; }
    }

    public class SelectionStatusEqualityComparer : IEqualityComparer<SelectionStatus>
    {
        public bool Equals(SelectionStatus? x, SelectionStatus? y)
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

        public int GetHashCode([DisallowNull] SelectionStatus obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    }     
}

