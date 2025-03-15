using Swapify.Notifications.Models;

namespace Swapify.Notifications.Services;

public interface IEmailSenderService
{
    Task SendEmailAsync(IAttachments attachments, string emailTemplate);
}