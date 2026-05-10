using ProjectTimeTracker.Application;
using ProjectTimeTracker.Infrastructure;
using ProjectTimeTracker.WebAPI.Extensions;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults("ProjectTimeTracker");

builder.Services.AddApplication();
builder.AddInfrastructure();

var app = builder.Build();

app.UseDefaults();

app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.MapGet("/users/me", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
})
    .RequireAuthorization();

app.Run();

