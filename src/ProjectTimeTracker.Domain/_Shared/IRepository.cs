namespace ProjectTimeTracker.Domain._Shared;

public interface IRepository<TEntity>
    where TEntity : AggregateRoot
{
    TEntity Add(TEntity entity);
    void Delete(TEntity entity);
}
