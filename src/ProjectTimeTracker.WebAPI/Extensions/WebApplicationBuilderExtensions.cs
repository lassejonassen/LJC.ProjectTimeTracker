using Carter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ProjectTimeTracker.Infrastructure.Security;
using ProjectTimeTracker.WebAPI.Security;

namespace ProjectTimeTracker.WebAPI.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddDefaults(this WebApplicationBuilder builder, string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            serviceName = builder.Environment.ApplicationName;

        builder.Services.AddTransient<IClaimsTransformation, PermissionClaimsTransformation>();
        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
        builder.Services.AddAuthorization(options =>
        {
            // Example Policy
            options.AddPolicy("CanWriteUsers", policy =>
                policy.AddRequirements(new PermissionRequirement("users:write")));

            options.AddPolicy("CanViewReports", policy =>
                policy.AddRequirements(new PermissionRequirement("reports:export")));
        });

        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<SecuritySchemeTransformer>();
        });
       
        builder.Services.AddCarter();

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddProblemDetails();

        builder.Services.AddCors();

        return builder;
    }
}
