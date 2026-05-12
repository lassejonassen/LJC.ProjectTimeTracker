using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace ProjectTimeTracker.Infrastructure.Security;

public class CustomAuthorizationHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // Check if the user is unauthenticated
        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "Unauthorized",
                Message = "You must be logged in to access this resource."
            });
            return;
        }

        // Check if the user is authenticated but lacks permissions
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "Forbidden",
                Message = "You do not have the required permissions."
            });
            return;
        }

        // Fallback to default behavior for successful requests
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
