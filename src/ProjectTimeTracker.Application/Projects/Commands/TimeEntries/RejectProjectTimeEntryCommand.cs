namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record RejectProjectTimeEntryCommand(Guid TimeEntryId) : IRequest<Result>;