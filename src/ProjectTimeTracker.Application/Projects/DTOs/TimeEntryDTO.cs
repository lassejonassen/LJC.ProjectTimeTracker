namespace ProjectTimeTracker.Application.Projects.DTOs;

public sealed record TimeEntryDTO
{
    public required Guid Id { get; init; }
    public required Guid ProjectId { get; init; }
    public required string UserId { get; init; }
    public string? Notes { get; init; }
    public required decimal Hours { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required DateTime UpdatedAtUtc { get; init; }
}
