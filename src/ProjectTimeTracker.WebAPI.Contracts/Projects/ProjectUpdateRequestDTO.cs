namespace ProjectTimeTracker.WebAPI.Contracts.Projects;

public sealed record ProjectUpdateRequestDTO(
    Guid ProjectId,
    string Name,
    string? Description);