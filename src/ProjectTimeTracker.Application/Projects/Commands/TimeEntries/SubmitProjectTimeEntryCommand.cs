namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record SubmitProjectTimeEntryCommand(Guid TimeEntryId) : IRequest<Result>;
