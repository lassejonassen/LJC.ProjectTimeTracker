using ProjectTimeTracker.Domain.Projects.Aggregates;

namespace ProjectTimeTracker.Domain.Projects.Repositories;
 
public interface IProjectRepository : IRepository<Project>
{
    Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken);
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
