namespace ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;

public sealed record ProjectTimeEntryCreateRequestDTO(Guid ProjectId, string? Notes, decimal Hours);
