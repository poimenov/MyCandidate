using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyModels.ComponentModel;

namespace MyCandidate.Common;

public class VacancyResource : ReactiveObject
{
    [Key]
    [ReadOnly(true)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Browsable(false)]
    public int Id { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Value")]
    public virtual string Value { get; set; } = string.Empty;

    [ForeignKey(nameof(VacancyId))]
    [Browsable(false)]
    public int VacancyId { get; set; }

    [Browsable(false)]
    public Vacancy? Vacancy { get; set; }

    [ForeignKey(nameof(ResourceTypeId))]
    [Browsable(false)]
    public int ResourceTypeId { get; set; }

    [Required]
    [DisplayName("Resource_Type")]
    public virtual ResourceType? ResourceType { get; set; }
}
