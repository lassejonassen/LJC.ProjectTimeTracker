namespace ProjectTimeTracker.WebAPI.Contracts.Projects;

public sealed record ProjectCreateRequestDTO(
    string Name,
    string? Description);