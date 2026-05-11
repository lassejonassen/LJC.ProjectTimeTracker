using ProjectTimeTracker.Domain.Authorization;

namespace ProjectTimeTracker.Infrastructure.Security;

internal sealed class RolePermissionAssignment
{
    private static readonly string[] ProjectMemberPermissions =
    [
        Permissions.Projects.Read,
        Permissions.Projects.ProjectTimeEntries.Read,
        Permissions.Projects.ProjectTimeEntries.Create,
        Permissions.Projects.ProjectTimeEntries.Update,
        Permissions.Projects.ProjectTimeEntries.Submit
    ];

    private static readonly string[] ProjectSupervisorPermissions =
    [
        ..ProjectMemberPermissions,
        Permissions.Projects.Update,
        Permissions.Projects.ProjectTimeEntries.Approve,
        Permissions.Projects.ProjectTimeEntries.Delete
    ];

    private static readonly string[] ProjectAdministratorPermissions =
    [
        ..ProjectSupervisorPermissions,
        Permissions.Projects.Create,
        Permissions.Projects.Delete,
        Permissions.Projects.ManageStatus
    ];

    public static Dictionary<string, string[]> RolePermissionAssignments { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        { Roles.ProjectMember, ProjectMemberPermissions },
        { Roles.ProjectSupervisor, ProjectSupervisorPermissions },
        { Roles.ProjectAdministrator, ProjectAdministratorPermissions }
    };
}