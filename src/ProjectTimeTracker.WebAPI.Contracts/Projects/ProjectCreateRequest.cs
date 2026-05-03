namespace ProjectTimeTracker.WebAPI.Contracts.Projects;

public sealed record ProjectCreateRequest(
    string Name,
    string? Description);