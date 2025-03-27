namespace Swapify.Notifications.Models;

public interface IAttachments
{
    string UserName { get; }
    string Email { get; }
    string? RedirectUrl { get; }
    string Subject { get; }
}