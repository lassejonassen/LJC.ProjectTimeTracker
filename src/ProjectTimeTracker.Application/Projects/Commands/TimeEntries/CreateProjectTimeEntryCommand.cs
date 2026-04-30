using ProjectTimeTracker.Domain.Projects.Errors;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Commands.TimeEntries;

public sealed record CreateProjectTimeEntryCommand(Guid ProjectId, string UserId, string? Notes, decimal Hours) : IRequest<Result>;

public sealed class CreateProjectTimeEntryCommandHandler(
    IProjectRepository repository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateProjectTimeEntryCommand, Result>
{
    public async Task<Result> Handle(CreateProjectTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        var result = project.AddTimeEntry(request.UserId, request.Notes, request.Hours, dateTimeProvider.UtcNow);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}