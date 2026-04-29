namespace ProjectTimeTracker.Domain._Shared.DomainEvents;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOnUtc { get; }
}
