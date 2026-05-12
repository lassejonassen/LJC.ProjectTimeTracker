using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ProjectTimeTracker.WebAPI.Security;

public class ConfigureJwtBearerOptions(IConfiguration configuration) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme && name != Options.DefaultName) return;

        var authSettings = configuration.GetSection("Authentication");

        options.RequireHttpsMetadata = false;
        options.Authority = authSettings["ValidIssuer"];
        options.Audience = authSettings["Audience"];
        options.MetadataAddress = authSettings["MetadataAddress"]!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = authSettings["ValidIssuer"],
            ValidAudience = authSettings["Audience"],
            // Keycloak specific: mapping the roles claim correctly
            RoleClaimType = "group"
        };
    }

    public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);
}
