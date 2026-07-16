namespace BJJManager.WebApi.Contracts.Techniques;

public record UpdateTechniqueRequest(string Name, string Position, string Description, IReadOnlyList<string> Steps);
