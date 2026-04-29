using ProjectTimeTracker.Domain._Shared.DomainEvents;

namespace ProjectTimeTracker.Domain._Shared;

public abstract class AggregateRoot : Entity, IHasDomainEvents
{
    public AggregateRoot() { }
    public AggregateRoot(DateTime utcNow) : base(utcNow) { }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
}
