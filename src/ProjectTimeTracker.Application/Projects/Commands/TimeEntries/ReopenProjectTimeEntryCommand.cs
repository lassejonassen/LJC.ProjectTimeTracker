namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record ReopenProjectTimeEntryCommand(Guid TimeEntryId) : IRequest<Result>;