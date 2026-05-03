namespace ProjectTimeTracker.WebAPI.Contracts.Projects.TimeEntries;

public sealed record ProjectTimeEntryStatusChangeRequest(Guid ProjectId, Guid TimeEntryId);