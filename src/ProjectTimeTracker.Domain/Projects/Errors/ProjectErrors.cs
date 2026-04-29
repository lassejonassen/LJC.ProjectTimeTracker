using ProjectTimeTracker.Domain.Projects.Aggregates;
using ProjectTimeTracker.Domain.Projects.Entities;

namespace ProjectTimeTracker.Domain.Projects.Errors;

public static class ProjectErrors
{
    private const string Prefix = nameof(Project);

    public static readonly Error NameRequired
        = new($"{Prefix}.{nameof(NameRequired)}", "The name is required", ErrorType.Validation);

    public static readonly Error NameTooLong
        = new($"{Prefix}.{nameof(NameTooLong)}", $"The name must not exceed {Project.NameMaxLength} characters", ErrorType.Validation);

    public static readonly Error DescriptionTooLong
        = new($"{Prefix}.{nameof(DescriptionTooLong)}", $"The description must not exceed {Project.DescriptionMaxLength} characters", ErrorType.Validation);

    public static readonly Error AlreadyClosed
        = new($"{Prefix}.{nameof(AlreadyClosed)}", $"The project is already closed", ErrorType.Failure);

    public static readonly Error AlreadyOpen
        = new($"{Prefix}.{nameof(AlreadyOpen)}", $"The project is already open", ErrorType.Failure);

    public static readonly Error NotFound
        = new($"{Prefix}.{nameof(NotFound)}", $"The project was not found", ErrorType.Failure);



    /*
     * TIME ENTRY ERRORS
     */
    public static readonly Error TimeEntryNotesTooLong
       = new($"{Prefix}.{nameof(TimeEntryNotesTooLong)}", $"The notes must not exceed {TimeEntry.NotesMaxLength} characters", ErrorType.Validation);

    public static readonly Error TimeEntryInvalidStatus
        = new($"{Prefix}.{nameof(TimeEntryInvalidStatus)}", $"The specified status is invalid", ErrorType.Failure);

    public static readonly Error TimeEntryOnlyDraftInvalidStateChange
        = new($"{Prefix}.{nameof(TimeEntryOnlyDraftInvalidStateChange)}", $"The state transition is invalid: Only drafts can be submitted", ErrorType.Failure);

    public static readonly Error TimeEntrySubmittedEntryInvalidStateChange
        = new($"{Prefix}.{nameof(TimeEntrySubmittedEntryInvalidStateChange)}", $"The state transition is invalid: Submitted entries must be Approved or Rejected", ErrorType.Failure);
    
    public static readonly Error TimeEntryApprovedEntriesAreFinal
       = new($"{Prefix}.{nameof(TimeEntryApprovedEntriesAreFinal)}", $"The state transition is invalid: Approved entries are final and cannot be changed", ErrorType.Failure);

    public static readonly Error TimeEntryRejectedEntriesMustBeMovedToDraft
       = new($"{Prefix}.{nameof(TimeEntryApprovedEntriesAreFinal)}", $"The state transition is invalid: Rejected entries must be moved back to Draft to be edited", ErrorType.Failure);

}
