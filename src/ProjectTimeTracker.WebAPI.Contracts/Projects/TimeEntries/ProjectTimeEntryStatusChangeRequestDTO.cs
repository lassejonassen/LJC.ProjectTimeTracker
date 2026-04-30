namespace ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;

public sealed record ProjectTimeEntryStatusChangeRequestDTO(Guid ProjectId, Guid TimeEntryId);