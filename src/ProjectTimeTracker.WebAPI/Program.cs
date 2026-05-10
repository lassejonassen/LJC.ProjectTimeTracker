using ProjectTimeTracker.Application;
using ProjectTimeTracker.Infrastructure;
using ProjectTimeTracker.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaults("ProjectTimeTracker");

builder.Services.AddApplication();
builder.AddInfrastructure();

var app = builder.Build();

app.UseDefaults();

app.MapGet("/", () => "Healthy");

app.Run();

