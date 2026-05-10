using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ProjectTimeTracker.WebAPI.Security;

public class SecuritySchemeTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        string authUrl = configuration["Authentication:AuthorizationUrl"]
                      ?? throw new InvalidOperationException("AuthorizationUrl is missing");

        string tokenUrl = configuration["Authentication:TokenUrl"]
                       ?? throw new InvalidOperationException("TokenUrl is missing");

        var scopes = configuration.GetSection("Authentication:Scopes")
                                  .Get<Dictionary<string, string>>()
                                  ?? new Dictionary<string, string>();

        document.Components.SecuritySchemes.Add("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(authUrl),
                    TokenUrl = new Uri(tokenUrl),
                    Scopes = scopes
                }
            }
        });

        document.Security = [
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("oauth2"),
                    scopes.Keys.ToList()
                }
            }
        ];

        document.SetReferenceHostDocument();

        return Task.CompletedTask;
    }
}
