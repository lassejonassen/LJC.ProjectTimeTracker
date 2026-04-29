using ProjectTimeTracker.Domain.Projects.Aggregates;
using ProjectTimeTracker.Domain.Projects.Enums;
using ProjectTimeTracker.Domain.Projects.Errors;

namespace ProjectTimeTracker.Domain.Projects.Entities;

public sealed class TimeEntry : Entity
{
    public const int NotesMaxLength = 500;

    private TimeEntry() { }
    private TimeEntry(DateTime utcNow) : base(utcNow) { }

    public Guid ProjectId { get; private set; }
    public Project Project { get; } = null!;
    public string UserId { get; private set; } = string.Empty; // For Keycloak Integration
    public string? Notes { get; private set; }
    public decimal Hours { get; private set; }
    public TimeEntryStatus Status { get; private set; }

    public static Result<TimeEntry> Create(Guid projectId, string userId, string? notes, decimal hours, DateTime utcNow)
    {
        var validationResult = ValidateInvariants(notes);
        if (validationResult.IsFailure)
            return Result.Failure<TimeEntry>(validationResult.Error);

        var timeEntry = new TimeEntry(utcNow)
        {
            ProjectId = projectId,
            UserId = userId,
            Notes = notes,
            Hours = hours,
            Status = TimeEntryStatus.Draft,
        };

        return Result.Success(timeEntry);
    }

    public Result Update(string? notes, decimal hours)
    {
        var validationResult = ValidateInvariants(notes);
        if (validationResult.IsFailure)
            return Result.Failure<TimeEntry>(validationResult.Error);

        Notes = notes;
        Hours = hours;

        return Result.Success();
    }

    public Result Submit()
    {
        var validationResult = ValidateStatusTransition(TimeEntryStatus.Submitted);
        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        Status = TimeEntryStatus.Submitted;

        return Result.Success();
    }

    public Result Approve()
    {
        var validationResult = ValidateStatusTransition(TimeEntryStatus.Approved);
        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        Status = TimeEntryStatus.Approved;

        return Result.Success();
    }

    public Result Reject()
    {
        var validationResult = ValidateStatusTransition(TimeEntryStatus.Rejected);
        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        Status = TimeEntryStatus.Rejected;

        return Result.Success();
    }

    public Result Reopen()
    {
        var validationResult = ValidateStatusTransition(TimeEntryStatus.Draft);
        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        Status = TimeEntryStatus.Draft;

        return Result.Success();
    }

    private static Result ValidateInvariants(string? notes)
    {
        if (notes is not null && notes.Length > NotesMaxLength)
            return Result.Failure(ProjectErrors.TimeEntryNotesTooLong);

        return Result.Success();
    }

    private Result<TimeEntryStatus> ValidateStatusTransition(TimeEntryStatus status)
    {
        if (Status ==  status)
        {
            return Result<TimeEntryStatus>.Success(status);
        }

        return Status switch
        {
            TimeEntryStatus.Draft => status == TimeEntryStatus.Submitted
            ? Result.Success(status)
            : Result.Failure<TimeEntryStatus>(ProjectErrors.TimeEntryOnlyDraftInvalidStateChange),

            TimeEntryStatus.Submitted => (status == TimeEntryStatus.Approved || status == TimeEntryStatus.Rejected)
                ? Result.Success(status)
                : Result.Failure<TimeEntryStatus>(ProjectErrors.TimeEntrySubmittedEntryInvalidStateChange),

            TimeEntryStatus.Approved => Result.Failure<TimeEntryStatus>(ProjectErrors.TimeEntryApprovedEntriesAreFinal),

            TimeEntryStatus.Rejected => status == TimeEntryStatus.Draft
                ? Result.Success(status) // Logical addition: Allow fix-and-resubmit
                : Result.Failure<TimeEntryStatus>(ProjectErrors.TimeEntryRejectedEntriesMustBeMovedToDraft),

            _ => Result.Failure<TimeEntryStatus>(ProjectErrors.TimeEntryInvalidStatus)
        };
    }

    private Result<TimeEntryStatus> GetStatus(string status)
    {
        return status?.Trim().ToLower() switch
        {
            "draft" => Result.Success(TimeEntryStatus.Draft),
            "submitted" => Result.Success(TimeEntryStatus.Submitted),
            "approved" => Result.Success(TimeEntryStatus.Approved),
            "rejected" => Result.Success(TimeEntryStatus.Rejected),
            _ => Result.Failure<TimeEntryStatus>(ProjectErrors.TimeEntryInvalidStatus)
        };
    }
}
