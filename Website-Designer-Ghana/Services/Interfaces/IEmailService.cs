using Website_Designer_Ghana.Services.Models;

namespace Website_Designer_Ghana.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
    Task SendContactFormEmailAsync(string customerEmail, string customerName, string subject, string message);
    Task SendContactConfirmationAsync(string customerEmail, string customerName);
    Task SendAdminNotificationAsync(string subject, string body);
}
