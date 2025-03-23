using Microsoft.Extensions.DependencyInjection;
using Swapify.API.Policies;
using Validation;

namespace Swapify.Api;

public static class ApplicationBuilderExtension
{
    public static IServiceCollection ApiConfigurations(
        this IServiceCollection services, 
        string issuer, string audience, string apiScope, string apiSecret, string clientSecret)
    {
        Requires.NotNull(services, nameof(services));
        Requires.NotNull(issuer, nameof(issuer));
        Requires.NotNull(audience, nameof(audience));
        Requires.NotNull(apiScope, nameof(apiScope));
        Requires.NotNull(apiSecret, nameof(apiSecret));
        Requires.NotNull(clientSecret, nameof(clientSecret));

        TokenOptions tokenOptions = new()
        {
             Issuer = issuer,
             Audience = audience,
             TokenLifeTime = TimeSpan.FromHours(24)
        };

        ApiOptions apiOptions = new()
        {
            ApiScope = apiScope,
            ApiSecret = apiSecret,
            ClientSecret = clientSecret
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