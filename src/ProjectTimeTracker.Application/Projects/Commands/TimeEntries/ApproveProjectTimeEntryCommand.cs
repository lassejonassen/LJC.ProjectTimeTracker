namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record ApproveProjectTimeEntryCommand(Guid TimeEntryId) : IRequest<Result>;
