using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Filters;

namespace Swapify.Host.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "Swagger PKCE Flow",
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri("https://localhost:7188/connect/authorize", UriKind.Absolute),
                    TokenUrl = new Uri("https://localhost:7188/connect/token", UriKind.Absolute)
                }
            }
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                },
                new List<string>()
            }
        });

        options.SwaggerDoc("v1",
            new OpenApiInfo { Title = "Swapify V1", Version = "1.0" });

        options.ExampleFilters();

        var xmlFiles = Directory
            .GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly)
            .ToList();

        xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));
    }
}