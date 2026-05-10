using ProjectTimeTracker.Application.Abstractions.Security;

namespace ProjectTimeTracker.Infrastructure.Security;

internal sealed class RolePermissionAssignment
{
    private static readonly string[] ProjectMemberPermissions =
        [
            Permissions.ProjectsRead
        ];
    private static readonly string[] ProjectSupervisorPermissions = [
        ..ProjectMemberPermissions];
    private static readonly string[] ProjectAdministratorPermissions = [
        ..ProjectSupervisorPermissions];

    public static Dictionary<string, string[]> RolePermissionAssignments { get; } = new()
    {
        {Roles.ProjectMember, ProjectMemberPermissions },
        {Roles.ProjectSupervisor, ProjectSupervisorPermissions },
        {Roles.ProjectAdministrator, ProjectAdministratorPermissions }
    };
}
