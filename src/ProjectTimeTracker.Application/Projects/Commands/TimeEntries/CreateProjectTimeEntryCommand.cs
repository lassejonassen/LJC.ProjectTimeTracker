namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record CreateProjectTimeEntryCommand(Guid ProjectId) : IRequest<Result>;
