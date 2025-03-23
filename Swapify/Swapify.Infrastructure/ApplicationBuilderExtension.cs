using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Swapify.Contracts.Services;
using Swapify.Contracts.Stores;
using Swapify.Contracts.Transactions;
using Swapify.Contracts.Validators;
using Swapify.Infrastructure.Entities;
using Swapify.Infrastructure.Options;
using Swapify.Infrastructure.Services;
using Swapify.Infrastructure.Stores;
using Swapify.Infrastructure.Transactions;
using Swapify.Infrastructure.Validators;
using Validation;
using UserStore = Swapify.Infrastructure.Stores.UserStore;

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
        services.AddTransient<IClientStore, ClientStore>();
        services.AddTransient<IRoleStore, RoleStore>();
        services.AddTransient<IUserRoleStore, UserRoleStore>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IUserRoleService, UserRoleService>();
        services.AddTransient<IEmailNotificationService, EmailNotificationService>();

        services.AddIdentityCore<UserEntity>().AddRoles<RoleEntity>();
        services.AddIdentity<UserEntity, RoleEntity>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserStore<UserStore<UserEntity, RoleEntity, ApplicationDbContext, string>>()
            .AddRoleStore<RoleStore<RoleEntity, ApplicationDbContext, string>>()
            .AddDefaultTokenProviders();
        services.AddTransient<IUserManagerService, UserManagerService>();

        return services;
    }
}