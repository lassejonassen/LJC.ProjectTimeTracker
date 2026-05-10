
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
        ];
    private static readonly string[] ProjectSupervisorPermissions = [
        ..ProjectMemberPermissions];
    private static readonly string[] ProjectAdministratorPermissions = [
        ..ProjectSupervisorPermissions,
        Permissions.Projects.Delete
        ];

    public static Dictionary<string, string[]> RolePermissionAssignments { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        {Roles.ProjectMember, ProjectMemberPermissions },
        {Roles.ProjectSupervisor, ProjectSupervisorPermissions },
        {Roles.ProjectAdministrator, ProjectAdministratorPermissions }
    };
}
