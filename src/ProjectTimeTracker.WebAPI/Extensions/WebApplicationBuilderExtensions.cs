using Carter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
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
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
        builder.Services.AddAuthorization();

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
