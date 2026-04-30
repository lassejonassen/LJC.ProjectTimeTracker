using ProjectTimeTracker.Domain.Projects.Errors;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record ApproveProjectTimeEntryCommand(Guid ProjectId, Guid TimeEntryId) : IRequest<Result>;

public sealed class ApproveProjectTimeEntryCommandHandler(
    IProjectRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ApproveProjectTimeEntryCommand, Result>
{
    public async Task<Result> Handle(ApproveProjectTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        var result = project.ApproveTimeEntry(request.TimeEntryId);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}