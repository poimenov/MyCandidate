using PropertyModels.ComponentModel;

namespace MyCandidate.Common.Interfaces;

public abstract class Entity : ReactiveObject
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual bool Enabled { get; set; }
}
