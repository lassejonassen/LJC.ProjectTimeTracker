namespace ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;

public sealed record ProjectTimeEntryUpdateRequest(
    Guid ProjectId,
    Guid TimeEntryId,
    string? Notes,
    decimal Hours);