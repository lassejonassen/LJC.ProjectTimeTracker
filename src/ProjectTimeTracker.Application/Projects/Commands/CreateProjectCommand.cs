using ProjectTimeTracker.Domain.Projects.Aggregates;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Commands;

public sealed record CreateProjectCommand(string Name, string? Description) : IRequest<Result<Guid>>;

public sealed class CreateProjectCommandHandler(
    IProjectRepository repository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = Project.Create(request.Name, request.Description, dateTimeProvider.UtcNow);

        if (project.IsFailure)
            return Result.Failure<Guid>(project.Error);

        repository.Add(project.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(project.Value.Id);
    }
}