using ProjectTimeTracker.Application.Projects.DTOs;
using ProjectTimeTracker.Domain.Projects.Errors;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Queries;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<Result<ProjectDTO>>;

public sealed class GetProjectByIdQueryHandler(
    IProjectRepository repository)
    : IRequestHandler<GetProjectByIdQuery, Result<ProjectDTO>>
{
    public async Task<Result<ProjectDTO>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (project is null)
            return Result.Failure<ProjectDTO>(ProjectErrors.NotFound);

        return Result.Success(new ProjectDTO
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status.ToString(),
            CreatedAtUtc = project.CreatedAtUtc,
            UpdatedAtUtc = project.UpdatedAtUtc
        });
    }
}