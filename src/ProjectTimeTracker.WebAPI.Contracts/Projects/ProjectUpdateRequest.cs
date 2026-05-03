namespace ProjectTimeTracker.WebAPI.Contracts.Projects;

public sealed record ProjectUpdateRequest(
    Guid ProjectId,
    string Name,
    string? Description);