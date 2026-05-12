using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectTimeTracker.Application.Abstractions.Interfaces;
using ProjectTimeTracker.Domain.Authorization;
using ProjectTimeTracker.Domain.Projects.Repositories;
using ProjectTimeTracker.Infrastructure.Persistence.DbContexts;
using ProjectTimeTracker.Infrastructure.Persistence.Interceptors;
using ProjectTimeTracker.Infrastructure.Persistence.Repositories;
using ProjectTimeTracker.Infrastructure.Security;
using ProjectTimeTracker.Infrastructure.Services;
using Serilog;

namespace ProjectTimeTracker.Infrastructure;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationHandler>();

        builder.AddCustomAuthorization();
        builder.AddLogging();
        builder.AddPersistence();


        return builder;
    }

    private static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
    {
        string? connectionString = builder.Configuration.GetConnectionString("Database");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "The connection string to the database is not set");
        }

        builder.Services.AddSingleton<SetUpdatedAtInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, opt) =>
        {
            opt.UseSqlServer(connectionString, x =>
            {
                x.EnableRetryOnFailure();
                x.MigrationsHistoryTable("__EFMigrationsHistory");
            });

            if (sp.GetRequiredService<IHostEnvironment>().IsDevelopment())
            {
                opt.EnableSensitiveDataLogging();
            }

            opt.AddInterceptors(sp.GetRequiredService<SetUpdatedAtInterceptor>());
        });

        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();


        return builder;
    }

    private static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        // 1. Setup the "Bootstrap" logger for startup failures
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        // 2. Use Serilog and read from appsettings.json
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        return builder;
    }

    private static WebApplicationBuilder AddCustomAuthorization(this WebApplicationBuilder builder)
    {
        var authorizationBuilder = builder.Services.AddAuthorizationBuilder();

        var permissions = Permissions.GetRegisteredPermissions();

        foreach (string permission in permissions)
        {
            authorizationBuilder.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
        }

        builder.Services.AddTransient<IClaimsTransformation, PermissionClaimsTransformation>();
        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return builder;
    }
}
