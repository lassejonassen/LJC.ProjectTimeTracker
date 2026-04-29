namespace ProjectTimeTracker.Domain._Shared.DomainEvents;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}