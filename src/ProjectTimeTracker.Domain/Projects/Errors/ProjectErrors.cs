using ProjectTimeTracker.Domain.Projects.Aggregates;

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
}
