using BJJManager.Domain.Common;

namespace BJJManager.Domain.Entities;

public class TechniqueStep : Entity
{
    public Guid TechniqueId { get; private set; }
    public int Order { get; private set; }
    public string Description { get; private set; }

    public TechniqueStep(Guid techniqueId, int order, string description)
        : base(Guid.NewGuid())
    {
        TechniqueId = techniqueId;
        Order = order;
        Description = description;
    }
}
