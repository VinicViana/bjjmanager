namespace BJJManager.WebApi.Contracts.Techniques;

public record CreateTechniqueRequest(string Name, string Position, string Description, IReadOnlyList<string> Steps);
