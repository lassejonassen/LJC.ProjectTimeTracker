using ProjectTimeTracker.Domain.Projects.Errors;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Commands;

public sealed record DeleteProjectCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteProjectCommandHandler(
    IProjectRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (project is null)
            return Result.Failure(ProjectErrors.NotFound);

        repository.Delete(project);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
