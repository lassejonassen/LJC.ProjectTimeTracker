using ProjectTimeTracker.Domain.Projects.Errors;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record RejectProjectTimeEntryCommand(Guid ProjectId, Guid TimeEntryId) : IRequest<Result>;

public sealed class RejectProjectTimeEntryCommandHandler(
    IProjectRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RejectProjectTimeEntryCommand, Result>
{
    public async Task<Result> Handle(RejectProjectTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        var result = project.RejectTimeEntry(request.TimeEntryId);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}