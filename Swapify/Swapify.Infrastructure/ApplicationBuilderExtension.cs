using Microsoft.Extensions.DependencyInjection;
using Swapify.Contracts.Services;
using Swapify.Contracts.Stores;
using Swapify.Contracts.Transactions;
using Swapify.Contracts.Validators;
using Swapify.Infrastructure.Options;
using Swapify.Infrastructure.Services;
using Swapify.Infrastructure.Stores;
using Swapify.Infrastructure.Transactions;
using Swapify.Infrastructure.Validators;
using Validation;

namespace Swapify.Infrastructure;

public static class ApplicationBuilderExtension
{
    public static IServiceCollection InfrastructureConfigurations(
        this IServiceCollection services,
        string defaultConnection)
    {
        Requires.NotNull(services, nameof(services));
        Requires.NotNull(defaultConnection, nameof(defaultConnection));

        SqlServerOptions sqlServerOptions = new()
        {
            DefaultConnection = defaultConnection
        };
        services.AddSingleton(sqlServerOptions);
        services.AddSingleton<IAtomicScopeFactory, AtomicScopeFactory>();

        services.AddTransient<IRoleValidator, RoleValidator>();
        services.AddTransient<IContextService, ContextService>();
        services.AddTransient<IUserStore, UserStore>();
        services.AddTransient<IRoleStore, RoleStore>();
        services.AddTransient<IUserRoleStore, UserRoleStore>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IUserRoleService, UserRoleService>();
        services.AddSingleton<IEncryptDecryptService>(new EncryptDecryptService("2E5A3789F8B9C4D2"));

        return services;
    }
}