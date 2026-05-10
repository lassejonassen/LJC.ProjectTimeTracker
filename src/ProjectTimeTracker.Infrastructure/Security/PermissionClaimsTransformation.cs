using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ProjectTimeTracker.Infrastructure.Security;

public class PermissionClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity!;
        if (identity.HasClaim(c => c.Type == "permission")) return Task.FromResult(principal);

        var groups = principal.FindAll("group").Select(c => c.Value);
        var permissions = MapGroupsToPermissions(groups);

        foreach (var permission in permissions)
        {
            identity.AddClaim(new Claim("permission", permission));
        }

        return Task.FromResult(principal);
    }

    private IEnumerable<string> MapGroupsToPermissions(IEnumerable<string> groups)
    {
        // Define your mapping logic here
        var map = new Dictionary<string, string[]>
        {
            { "Admins", new[] { "users:read", "users:write", "reports:export" } },
            { "Managers", new[] { "users:read", "reports:export" } },
            { "Viewers", new[] { "users:read" } }
        };

        return groups.SelectMany(g => map.GetValueOrDefault(g, Array.Empty<string>())).Distinct();
    }
}
