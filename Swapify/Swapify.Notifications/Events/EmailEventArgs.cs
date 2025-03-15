using Swapify.Notifications.Models;

namespace Swapify.Notifications.Events;

public class EmailEventArgs : EventArgs
{
    public IAttachments Attachments { get; }
    public string Template { get; }

    public EmailEventArgs(IAttachments attachments, string template)
    {
        Attachments = attachments;
        Template = template;
    }
}