namespace Swapify.Notifications.Models;

public interface IAttachments
{
    string? FirstName { get; }
    string? LastName { get; }
    string Email { get; }
    string? RedirectUrl { get; }
    string Subject { get; }
}