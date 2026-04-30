using ProjectTimeTracker.Domain.Projects.Aggregates;
using ProjectTimeTracker.Domain.Projects.Repositories;
using ProjectTimeTracker.Infrastructure.Persistence.DbContexts;

namespace ProjectTimeTracker.Infrastructure.Persistence.Repositories;

internal sealed class ProjectRepository(ApplicationDbContext context)
    : Repository<Project>(context), IProjectRepository
{
    public async Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await DbContext.Set<Project>().ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await DbContext.Set<Project>()
           .Include(x => x.TimeEntries)
           .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
