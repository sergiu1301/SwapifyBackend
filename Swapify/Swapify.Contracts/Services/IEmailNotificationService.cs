using System.Net.Mail;

namespace Swapify.Contracts.Services;

public interface IEmailNotificationService
{
    public Task SendEmailConfirmationAsync(string username, string userId, string userEmail, string encodedToken);

    public Task SendForgotPasswordAsync(string username, string userId, string userEmail, string encodedToken);

    public Task SendBlockAccountAsync(string username, string userEmail);

    public Task SendUnblockAccountAsync(string username, string userEmail);
}