using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Filters;

namespace Swapify.Host.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                Array.Empty<string>()
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