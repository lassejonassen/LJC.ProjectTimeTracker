using Microsoft.AspNetCore.Authorization;

namespace ProjectTimeTracker.Infrastructure.Security;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
