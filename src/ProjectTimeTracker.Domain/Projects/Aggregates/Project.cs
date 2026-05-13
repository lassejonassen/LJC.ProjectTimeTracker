using ProjectTimeTracker.Domain.Projects.Entities;
using ProjectTimeTracker.Domain.Projects.Enums;
using ProjectTimeTracker.Domain.Projects.Errors;

namespace ProjectTimeTracker.Domain.Projects.Aggregates;

public sealed class Project : AggregateRoot
{
    public const int NameMaxLength = 50;
    public const int DescriptionMaxLength = 255;

    private Project() { }
    private Project(DateTime utcNow) : base(utcNow) { }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }

    private readonly List<TimeEntry> _timeEntries = [];
    public IReadOnlyCollection<TimeEntry> TimeEntries => _timeEntries.AsReadOnly();

    public static Result<Project> Create(string name, string? description, DateTime utcNow)
    {
        var validationResult = ValidateInvariants(name, description);
        if (validationResult.IsFailure)
            return Result.Failure<Project>(validationResult.Error);

        var project = new Project(utcNow)
        {
            Name = name,
            Description = description,
            Status = ProjectStatus.Open
        };

        return Result.Success(project);
    }

    public Result Update(string name, string? description)
    {
        var validationResult = ValidateInvariants(name, description);
        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        Name = name;
        Description = description;

        return Result.Success();
    }

    public Result Close()
    {
        if (Status != ProjectStatus.Open)
            return Result.Failure(ProjectErrors.AlreadyClosed);

        Status = ProjectStatus.Closed;

        return Result.Success();
    }

    public Result Reopen()
    {
        if (Status != ProjectStatus.Closed)
            return Result.Failure(ProjectErrors.AlreadyOpen);

        Status = ProjectStatus.Open;

        return Result.Success();
    }

    private static Result ValidateInvariants(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ProjectErrors.NameRequired);

        if (name.Length > NameMaxLength)
            return Result.Failure(ProjectErrors.NameTooLong);

        if (description is not null && description.Length > DescriptionMaxLength)
            return Result.Failure(ProjectErrors.DescriptionTooLong);

        return Result.Success();
    }

    public Result AddTimeEntry(string userId, string? notes, decimal hours, DateTime utcNow)
    {
        var timeEntry = TimeEntry.Create(
            Id,
            userId,
            notes,
            hours, utcNow);

        if (timeEntry.IsFailure)
            return Result.Failure(timeEntry.Error);

        _timeEntries.Add(timeEntry.Value);

        return Result.Success();
    }

    public Result UpdateTimeEntry(Guid timeEntryId, string? notes, decimal hours)
    {
        var timeEntry = _timeEntries.FirstOrDefault(x => x.Id == timeEntryId);

        if (timeEntry is null)
            return Result.Failure(ProjectErrors.TimeEntryNotFound);

        var result = timeEntry.Update(notes, hours);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        return Result.Success();
    }

    public Result SubmitTimeEntry(Guid timeEntryId)
    {
        var timeEntry = _timeEntries.FirstOrDefault(x => x.Id == timeEntryId);

        if (timeEntry is null)
            return Result.Failure(ProjectErrors.TimeEntryNotFound);

        timeEntry.Submit();

        return Result.Success();
    }

    public Result ApproveTimeEntry(Guid timeEntryId)
    {
        var timeEntry = _timeEntries.FirstOrDefault(x => x.Id == timeEntryId);

        if (timeEntry is null)
            return Result.Failure(ProjectErrors.TimeEntryNotFound);

        timeEntry.Approve();

        return Result.Success();
    }

    public Result RejectTimeEntry(Guid timeEntryId)
    {
        var timeEntry = _timeEntries.FirstOrDefault(x => x.Id == timeEntryId);

        if (timeEntry is null)
            return Result.Failure(ProjectErrors.TimeEntryNotFound);

        timeEntry.Reject();
        return Result.Success();
    }

    public Result ReopenTimeEntry(Guid timeEntryId)
    {
        var timeEntry = _timeEntries.FirstOrDefault(x => x.Id == timeEntryId);

        if (timeEntry is null)
            return Result.Failure(ProjectErrors.TimeEntryNotFound);

        timeEntry.Reopen();

        return Result.Success();
    }

    public Result RemoveTimeEntry(Guid timeEntryId)
    {
        var timeEntry = _timeEntries.FirstOrDefault(x => x.Id == timeEntryId);

        if (timeEntry is null)
            return Result.Failure(ProjectErrors.TimeEntryNotFound);

        _timeEntries.Remove(timeEntry);

        return Result.Success();
    }
}
