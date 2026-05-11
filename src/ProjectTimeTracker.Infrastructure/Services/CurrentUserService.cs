using Microsoft.AspNetCore.Http;
using ProjectTimeTracker.Application.Abstractions.Interfaces;
using System.Security.Claims;

namespace ProjectTimeTracker.Infrastructure.Services;

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue("sub") ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Username => User?.FindFirstValue("username") ?? "UNKNOWN_USERNAME";

    public string? Email => User?.FindFirstValue("email") ?? User?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles =>
        User?.FindAll("group").Select(c => c.Value) ?? [];
}
