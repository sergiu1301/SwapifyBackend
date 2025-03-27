namespace Swapify.Notifications.Models;

public class Attachments : IAttachments
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? RedirectUrl { get; set;  }
    public string Subject { get; set; }
}