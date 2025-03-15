using Microsoft.Extensions.DependencyInjection;
using Swapify.API.Policies;
using Validation;

namespace Swapify.Api;

public static class ApplicationBuilderExtension
{
    public static IServiceCollection ApiConfigurations(
        this IServiceCollection services, 
        IDictionary<string, string> options)
    {
        Requires.NotNull(services, nameof(services));
        Requires.NotNull(options, nameof(options));

        TokenOptions tokenOptions = new()
        {
             Issuer = options["Issuer"],
             Audience = options["Audience"],
             TokenLifeTime = TimeSpan.FromHours(24),
             GoogleClientId = options["GoogleClientId"]
        };
        ApiOptions apiOptions = new()
        {
            ApiScope = options["ApiScope"],
            ApiSecret = options["ApiSecret"]
        };
        services.AddSingleton(tokenOptions);
        services.AddSingleton(apiOptions);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AdminPolicy, builder => builder.RequireRole(Roles.AdminRole));
        });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.UserPolicy, builder => builder.RequireRole(Roles.UserRole));
        });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.UserOrAdminPolicy, builder =>
            {
                builder.RequireAssertion(context => context.User.IsInRole(Roles.AdminRole) || context.User.IsInRole(Roles.UserRole));
            });
        });

        return services;
    }
}