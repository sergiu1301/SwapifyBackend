using System.Net.Mail;

namespace Swapify.Contracts.Services;

public interface IEmailNotificationService
{
    public Task SendEmailConfirmationAsync(string? firstName, string? lastName, string userId, string userEmail, string encodedToken);

    public Task SendForgotPasswordAsync(string? firstName, string? lastName, string userId, string userEmail, string encodedToken);

    public Task SendBlockAccountAsync(string? firstName, string? lastName, string userEmail);

    public Task SendUnblockAccountAsync(string? firstName, string? lastName, string userEmail);
}