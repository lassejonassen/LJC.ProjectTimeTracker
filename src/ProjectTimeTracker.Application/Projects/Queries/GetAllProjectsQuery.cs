using ProjectTimeTracker.Application.Projects.DTOs;
using ProjectTimeTracker.Domain.Projects.Repositories;

namespace ProjectTimeTracker.Application.Projects.Queries;

public sealed record GetAllProjectsQuery : IRequest<IReadOnlyCollection<ProjectDTO>>;

public sealed class GetAllProjectsQueryHandler(
    IProjectRepository repository)
    : IRequestHandler<GetAllProjectsQuery, IReadOnlyCollection<ProjectDTO>>
{
    public async Task<IReadOnlyCollection<ProjectDTO>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await repository.GetAllAsync(cancellationToken);

        return [.. projects.Select(x => new ProjectDTO {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Status = x.Status.ToString(),
            CreatedAtUtc = x.CreatedAtUtc,
            UpdatedAtUtc = x.UpdatedAtUtc,
        })];
    }
}