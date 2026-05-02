using ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;

namespace ProjectTimeTracker.WebAPI.Contracts.Projects;

public sealed record ProjectResponseDTO
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required DateTime UpdatedAtUtc { get; init; }
    public IReadOnlyCollection<TimeEntryResponseDTO>? TimeEntries { get; init; }
}
