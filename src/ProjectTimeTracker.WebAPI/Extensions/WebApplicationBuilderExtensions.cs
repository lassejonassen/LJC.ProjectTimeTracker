using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProjectTimeTracker.WebAPI.Security;

namespace ProjectTimeTracker.WebAPI.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddDefaults(this WebApplicationBuilder builder, string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            serviceName = builder.Environment.ApplicationName;

        builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();
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
