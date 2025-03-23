using System.Reflection;
using Swashbuckle.AspNetCore.Filters;
using Validation;

namespace Swapify.Host.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerExampleConfigurations(
        this IServiceCollection services)
    {
        Requires.NotNull(services, nameof(services));

        Assembly entryAssembly = Assembly.GetEntryAssembly();
        Assembly[] assemblies = entryAssembly.GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Concat(new[] { entryAssembly })
            .ToArray();

        return services.AddSwaggerExamplesFromAssemblies(assemblies);
    }
}