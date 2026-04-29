using Microsoft.EntityFrameworkCore.Diagnostics;
using ProjectTimeTracker.Domain._Shared;

namespace ProjectTimeTracker.Infrastructure.Persistence.Interceptors;

public sealed class SetUpdatedAtInterceptor : SaveChangesInterceptor
{
    public SetUpdatedAtInterceptor()
    {
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            SetUpdatedAt(eventData.Context, DateTime.UtcNow);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetUpdatedAt(DbContext context, DateTime utcNow)
    {
        var entities = context
            .ChangeTracker
            .Entries<Entity>()
            .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified)
            .Select(entry => entry.Entity)
            .ToList();

        foreach (var entity in entities)
        {
            entity.UpdatedAtUtc = utcNow;
        }
    }
}