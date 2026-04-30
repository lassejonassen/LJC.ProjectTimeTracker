namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record UpdateProjectTimeEntryCommand(Guid ProjectId) : IRequest<Result>;
