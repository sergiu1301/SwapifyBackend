using Swapify.Notifications.Models;

namespace Swapify.Notifications.Events;

public class EmailEventPublisher
{
    public event EventHandler<EmailEventArgs> EmailEvent;

    public void RaiseEmailEvent(IAttachments attachments, string template)
    {
        EmailEvent?.Invoke(this, new EmailEventArgs(attachments, template));
    }
}