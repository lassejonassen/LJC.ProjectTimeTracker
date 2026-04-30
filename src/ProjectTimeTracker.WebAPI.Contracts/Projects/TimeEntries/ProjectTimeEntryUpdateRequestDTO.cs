namespace ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;

public sealed record ProjectTimeEntryUpdateRequestDTO(
    Guid ProjectId,
    Guid TimeEntryId,
    string? Notes,
    decimal Hours);