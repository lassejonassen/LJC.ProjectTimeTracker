using ProjectTimeTracker.Domain.Projects.Errors;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record DeleteProjectTimeEntryCommand(Guid ProjectId, Guid TimeEntryId) : IRequest<Result>;

public sealed class DeleteProjectTimeEntryCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProjectTimeEntryCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        var result = project.RemoveTimeEntry(request.TimeEntryId);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}