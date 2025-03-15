using Microsoft.Extensions.Logging;
using Swapify.Notifications.Events;
using Swapify.Notifications.Services;

namespace Swapify.Notifications.Handlers;

public class EmailHandler
{
    private readonly ILogger<EmailHandler> _logger;
    private readonly IEmailSenderService _emailService;

    public EmailHandler(IEmailSenderService emailService, ILogger<EmailHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async void HandleEmailEvent(object sender, EmailEventArgs e)
    {
        try
        {
            await _emailService.SendEmailAsync(e.Attachments, e.Template);
        }
        catch (Exception ex)
        {
            _logger.LogError("Sending email failed with error: {Error}", ex.Message);
        }
    }
}