using ProjectTimeTracker.Domain.Projects.Errors;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record UpdateProjectTimeEntryCommand(Guid ProjectId, Guid TimeEntryId, string? Notes, decimal Hours) : IRequest<Result>;

public sealed class UpdateProjectTimeEntryCommandHandler(
    IProjectRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProjectTimeEntryCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        var result = project.UpdateTimeEntry(request.TimeEntryId, request.Notes, request.Hours);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}