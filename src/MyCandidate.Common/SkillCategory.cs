using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.Common
{
    public class SkillCategory : Entity
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Browsable(false)]
        public override int Id { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        [DisplayName("Category_Name")]
        public override string Name { get; set; }

        [Required]
        [DefaultValue(true)]
        public override bool Enabled { get; set; }

        [Browsable(false)]
        public virtual List<Skill> Skills { get; set; }
    }

    public class SkillCategoryEqualityComparer : IEqualityComparer<SkillCategory>
    {
        public bool Equals(SkillCategory? x, SkillCategory? y)
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

        public int GetHashCode([DisallowNull] SkillCategory obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Name.GetHashCode(), obj.Enabled.GetHashCode());
    }    
}
