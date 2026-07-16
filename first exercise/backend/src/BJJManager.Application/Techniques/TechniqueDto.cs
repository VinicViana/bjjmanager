using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;

namespace BJJManager.Application.Techniques;

public record TechniqueStepDto(Guid Id, int Order, string Description);

public record TechniqueMediaDto(Guid Id, string FileName, string FileUrl, MediaType MediaType);

public record TechniqueDto(
    Guid Id,
    string Name,
    string Position,
    string Description,
    IReadOnlyCollection<TechniqueStepDto> Steps,
    IReadOnlyCollection<TechniqueMediaDto> Media);

public static class TechniqueMappings
{
    public static TechniqueDto ToDto(this Technique technique) =>
        new(
            technique.Id,
            technique.Name,
            technique.Position,
            technique.Description,
            technique.Steps.OrderBy(s => s.Order).Select(s => s.ToDto()).ToList(),
            technique.Media.Select(m => m.ToDto()).ToList());

    public static TechniqueStepDto ToDto(this TechniqueStep step) =>
        new(step.Id, step.Order, step.Description);

    public static TechniqueMediaDto ToDto(this TechniqueMedia media) =>
        new(media.Id, media.FileName, media.FileUrl, media.MediaType);
}
