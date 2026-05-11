using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ProjectTimeTracker.Infrastructure.Security;

public class PermissionClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(c => c.Type == "permission"))
        {
            return Task.FromResult(principal);
        }

        List<string> permissions = [];

        if (principal.IsInRole(Roles.ProjectAdministrator))
            permissions.AddRange(RolePermissionAssignment.RolePermissionAssignments[Roles.ProjectAdministrator]);

        if (principal.IsInRole(Roles.ProjectSupervisor))
            permissions.AddRange(RolePermissionAssignment.RolePermissionAssignments[Roles.ProjectSupervisor]);

        if (principal.IsInRole(Roles.ProjectMember))
            permissions.AddRange(RolePermissionAssignment.RolePermissionAssignments[Roles.ProjectMember]);

        permissions = [.. permissions.Distinct()];

        var claimsIdentity = new ClaimsIdentity();

        foreach (var permission in permissions)
            claimsIdentity.AddClaim(new Claim("permission", permission));

        principal.AddIdentity(claimsIdentity);


        return Task.FromResult(principal);
    }
}
