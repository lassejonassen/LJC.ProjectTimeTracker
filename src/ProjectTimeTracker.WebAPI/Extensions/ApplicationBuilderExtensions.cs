using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Infrastructure.Persistence.DbContexts;

namespace ProjectTimeTracker.WebAPI.Extensions;

public static partial class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        if (env.IsDevelopment())
        {
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>()
                .Database.Migrate();
        }

        return app;
    }
}