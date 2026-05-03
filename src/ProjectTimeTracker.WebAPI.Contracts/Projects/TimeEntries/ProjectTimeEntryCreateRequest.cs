namespace ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;

public sealed record ProjectTimeEntryCreateRequest(Guid ProjectId, string? Notes, decimal Hours);
