using Carter;

namespace ProjectTimeTracker.WebAPI.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddDefaults(this WebApplicationBuilder builder, string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            serviceName = builder.Environment.ApplicationName;

        builder.Services.AddOpenApi();
        builder.Services.AddCarter();

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddProblemDetails();

        builder.Services.AddCors();

        return builder;
    }
}
