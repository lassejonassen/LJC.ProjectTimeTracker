using Carter;
using Scalar.AspNetCore;

namespace ProjectTimeTracker.WebAPI.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseDefaults(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Project Time Tracker")
            .WithTheme(ScalarTheme.Moon)
            .WithDocumentDownloadType(DocumentDownloadType.Json)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        app.UseHttpsRedirection();

        app.MapCarter();

        app.UseCors(opt =>
        {
            opt.AllowAnyHeader();
            opt.AllowAnyMethod();
            opt.AllowAnyOrigin();
        });

        app.ApplyDatabaseMigrations();

        return app;
    }
}