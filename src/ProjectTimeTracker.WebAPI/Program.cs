using ProjectTimeTracker.Application;
using ProjectTimeTracker.Infrastructure;
using ProjectTimeTracker.WebAPI.Extensions;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults("project-time-tracker-api");

builder.Services.AddApplication();
builder.AddInfrastructure();

var app = builder.Build();

app.UseDefaults();

app.MapGet("/", () => "Healthy");

app.MapGet("/auth-debug", (ClaimsPrincipal user) =>
    user.Claims.Select(c => new { c.Type, c.Value }));

app.Run();

