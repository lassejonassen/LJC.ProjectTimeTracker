using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace ProjectTimeTracker.WebAPI.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddDefaults(this WebApplicationBuilder builder, string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            serviceName = builder.Environment.ApplicationName;

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = builder.Configuration["Authentication:ValidIssuer"];
                options.Audience = builder.Configuration["Authentication:Audience"];
                options.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
                    RoleClaimType = "realm_access.roles"
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddOpenApi((options) =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes.Add("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(builder.Configuration["Authentication:AuthorizationUrl"]!),
                            TokenUrl = new Uri(builder.Configuration["Authentication:TokenUrl"]!),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID" },
                                { "profile", "Profile" }
                            //    { "api", "Access the API" },
                            //    { "email", "Access the user's email address" },
                            }
                        }
                    }
                });

                document.Security = [
                    new OpenApiSecurityRequirement
                    {
                        {
                            //new OpenApiSecuritySchemeReference("oauth2"),
                            //["api", "profile", "email", "openid"]
                            new OpenApiSecuritySchemeReference("oauth2"),
                            ["profile", "openid"]
                        }
                    }
                ];

                document.SetReferenceHostDocument();

                return Task.CompletedTask;
            });
        });
        builder.Services.AddCarter();

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddProblemDetails();

        builder.Services.AddCors();

        return builder;
    }
}
