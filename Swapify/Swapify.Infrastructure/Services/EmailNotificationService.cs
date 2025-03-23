using Azure.Core;
using Swapify.Contracts.Services;
using Swapify.Infrastructure.Models;
using Swapify.Notifications.Events;
using Swapify.Notifications.Handlers;
using Swapify.Notifications.Models;

namespace Swapify.Infrastructure.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly EmailEventPublisher _emailEventPublisher;

    private const string ClientBaseUrl = "http://localhost:5173";

    public EmailNotificationService(EmailEventPublisher emailEventPublisher, EmailHandler emailHandler)
    {
        _emailEventPublisher = emailEventPublisher;

        _emailEventPublisher.EmailEvent += emailHandler.HandleEmailEvent!;
    }

    public Task SendEmailConfirmationAsync(string? firstName, string? lastName, string userId, string userEmail, string encodedToken)
    {
        string confirmationLink = $"{ClientBaseUrl}/confirm-email?userId={userId}&token={encodedToken}";

        IAttachments emailEvent = new Attachments()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            RedirectUrl = confirmationLink,
            Subject = "Email Confirmation"
        };

        _emailEventPublisher.RaiseEmailEvent(emailEvent, "EmailConfirmationTemplate.html");

        return Task.CompletedTask;
    }

    public Task SendForgotPasswordAsync(string? firstName, string? lastName, string userId, string userEmail, string encodedToken)
    {
        string resetLink = $"{ClientBaseUrl}/reset-password?userId={userId}&token={encodedToken}";

        IAttachments emailEvent = new Attachments()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            RedirectUrl = resetLink,
            Subject = "Forgot Password"
        };

        _emailEventPublisher.RaiseEmailEvent(emailEvent, "ForgotPasswordTemplate.html");

        return Task.CompletedTask;
    }

    public Task SendBlockAccountAsync(string? firstName, string? lastName, string userEmail)
    {
        IAttachments emailEvent = new Attachments()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            Subject = "Block Account"
        };

        _emailEventPublisher.RaiseEmailEvent(emailEvent, "BlockAccountTemplate.html");

        return Task.CompletedTask;
    }

    public Task SendUnblockAccountAsync(string? firstName, string? lastName, string userEmail)
    {
        IAttachments emailEvent = new Attachments()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = userEmail,
            Subject = "Unblock Account"
        };

        _emailEventPublisher.RaiseEmailEvent(emailEvent, "UnblockAccountTemplate.html");

        return Task.CompletedTask;
    }
}