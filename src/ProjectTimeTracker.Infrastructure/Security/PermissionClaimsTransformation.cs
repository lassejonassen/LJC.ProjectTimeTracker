using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ProjectTimeTracker.Infrastructure.Security;

public class PermissionClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity!;
        if (identity.HasClaim(c => c.Type == "permission")) return Task.FromResult(principal);

        var groups = principal.Claims
            .Where(c => c.Type == "group" || c.Type == "Group" || c.Type.EndsWith("/group"))
            .Select(c => c.Value)
            .ToList();

        var permissions = MapGroupsToPermissions(groups);

        foreach (var permission in permissions)
        {
            identity.AddClaim(new Claim("permission", permission));
        }

        return Task.FromResult(principal);
    }

    private static IEnumerable<string> MapGroupsToPermissions(IEnumerable<string> groups)
    {
        var permissions = RolePermissionAssignment.RolePermissionAssignments;

        return [.. groups
            .Where(g => permissions.ContainsKey(g))
            .SelectMany(g => permissions[g])
            .Distinct()];
    }
}
