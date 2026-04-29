using ProjectTimeTracker.Domain._Shared;

namespace ProjectTimeTracker.Infrastructure.Persistence.Repositories;

internal abstract class Repository<TEntity>(DbContext context) : IRepository<TEntity>
    where TEntity : AggregateRoot
{
    protected DbContext DbContext => context;

    public TEntity Add(TEntity entity)
    {
        return context.Set<TEntity>().Add(entity).Entity;
    }

    public void Delete(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
    }
}