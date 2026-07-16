using BJJManager.Domain.Common;
using BJJManager.Domain.Enums;
using BJJManager.Domain.Exceptions;

namespace BJJManager.Domain.Entities;

public class Technique : Entity
{
    private readonly List<TechniqueStep> _steps = new();
    private readonly List<TechniqueMedia> _media = new();

    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string Position { get; private set; }
    public string Description { get; private set; }
    public IReadOnlyCollection<TechniqueStep> Steps => _steps.AsReadOnly();
    public IReadOnlyCollection<TechniqueMedia> Media => _media.AsReadOnly();

    private Technique()
        : base(Guid.NewGuid())
    {
        Name = string.Empty;
        Position = string.Empty;
        Description = string.Empty;
    }

    public Technique(Guid userId, string name, string position, string description, IReadOnlyList<string> stepDescriptions)
        : base(Guid.NewGuid())
    {
        RequireAtLeastOneStep(stepDescriptions);

        UserId = userId;
        Name = name;
        Position = position;
        Description = description;

        AppendSteps(stepDescriptions);
    }

    public void UpdateDetails(string name, string position, string description)
    {
        Name = name;
        Position = position;
        Description = description;
    }

    public void ReplaceSteps(IReadOnlyList<string> stepDescriptions)
    {
        RequireAtLeastOneStep(stepDescriptions);

        _steps.Clear();
        AppendSteps(stepDescriptions);
    }

    public TechniqueMedia AddMedia(string fileName, string fileUrl, MediaType mediaType)
    {
        var media = new TechniqueMedia(Id, fileName, fileUrl, mediaType);
        _media.Add(media);
        return media;
    }

    public void RemoveMedia(Guid mediaId)
    {
        _media.RemoveAll(m => m.Id == mediaId);
    }

    private void AppendSteps(IReadOnlyList<string> stepDescriptions)
    {
        for (var i = 0; i < stepDescriptions.Count; i++)
            _steps.Add(new TechniqueStep(Id, i + 1, stepDescriptions[i]));
    }

    private static void RequireAtLeastOneStep(IReadOnlyList<string> stepDescriptions)
    {
        if (stepDescriptions is null || stepDescriptions.Count == 0)
            throw new DomainException("A technique must contain at least one step.");
    }
}
