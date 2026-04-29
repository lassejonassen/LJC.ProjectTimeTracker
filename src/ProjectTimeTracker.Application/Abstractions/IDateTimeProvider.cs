namespace ProjectTimeTracker.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
