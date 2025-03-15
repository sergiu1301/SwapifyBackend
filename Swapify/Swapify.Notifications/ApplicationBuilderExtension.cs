using Microsoft.Extensions.DependencyInjection;
using Swapify.Notifications.Events;
using Swapify.Notifications.Handlers;
using Swapify.Notifications.Services;
using Validation;

namespace Swapify.Notifications;

public static class ApplicationBuilderExtension
{
    public static IServiceCollection NotificationConfigurations(
        this IServiceCollection services)
    {
        Requires.NotNull(services, nameof(services));

        services.AddTransient<IEmailSenderService, EmailSenderService>();
        services.AddTransient<EmailEventPublisher>();
        services.AddTransient<EmailHandler>();

        return services;
    }
}