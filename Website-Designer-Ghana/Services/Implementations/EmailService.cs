using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Website_Designer_Ghana.Services.Interfaces;
using Website_Designer_Ghana.Services.Models;

namespace Website_Designer_Ghana.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            email.To.Add(new MailboxAddress(message.ToName, message.To));
            email.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder();
            if (message.IsHtml)
            {
                bodyBuilder.HtmlBody = message.Body;
            }
            else
            {
                bodyBuilder.TextBody = message.Body;
            }

            email.Body = bodyBuilder.ToMessageBody();

            // Add CC recipients
            if (message.CcList != null && message.CcList.Any())
            {
                foreach (var cc in message.CcList)
                {
                    email.Cc.Add(MailboxAddress.Parse(cc));
                }
            }

            // Add BCC recipients
            if (message.BccList != null && message.BccList.Any())
            {
                foreach (var bcc in message.BccList)
                {
                    email.Bcc.Add(MailboxAddress.Parse(bcc));
                }
            }

            using var smtp = new SmtpClient();

            // Connect to SMTP server
            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort,
                _emailSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

            // Authenticate if credentials provided
            if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername))
            {
                await smtp.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            }

            // Send email
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", message.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.To);
            throw;
        }
    }

    public async Task SendContactFormEmailAsync(string customerEmail, string customerName, string subject, string message)
    {
        var emailBody = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #ef4444; color: white; padding: 20px; text-align: center; }}
                    .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
                    .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    .info {{ background-color: white; padding: 15px; border-left: 4px solid #ef4444; margin: 15px 0; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>New Contact Form Submission</h2>
                    </div>
                    <div class='content'>
                        <div class='info'>
                            <p><strong>From:</strong> {customerName}</p>
                            <p><strong>Email:</strong> {customerEmail}</p>
                            <p><strong>Subject:</strong> {subject}</p>
                        </div>
                        <div class='info'>
                            <p><strong>Message:</strong></p>
                            <p>{message.Replace("\n", "<br>")}</p>
                        </div>
                    </div>
                    <div class='footer'>
                        <p>This email was sent from Website Designer Ghana contact form.</p>
                    </div>
                </div>
            </body>
            </html>";

        await SendEmailAsync(new EmailMessage
        {
            To = _emailSettings.FromEmail, // Send to admin
            ToName = "Admin",
            Subject = $"New Contact Form: {subject}",
            Body = emailBody,
            IsHtml = true
        });
    }

    public async Task SendContactConfirmationAsync(string customerEmail, string customerName)
    {
        var emailBody = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #ef4444; color: white; padding: 20px; text-align: center; }}
                    .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 5px; margin-top: 20px; }}
                    .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Thank You for Contacting Us!</h2>
                    </div>
                    <div class='content'>
                        <p>Hello {customerName},</p>
                        <p>Thank you for reaching out to Website Designer Ghana. We have received your message and will get back to you as soon as possible.</p>
                        <p>Our team typically responds within 24-48 hours during business days.</p>
                        <p>If you have any urgent queries, please don't hesitate to call us directly.</p>
                        <br>
                        <p>Best regards,</p>
                        <p><strong>Website Designer Ghana Team</strong></p>
                    </div>
                    <div class='footer'>
                        <p>&copy; 2026 Website Designer Ghana. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";

        await SendEmailAsync(new EmailMessage
        {
            To = customerEmail,
            ToName = customerName,
            Subject = "Thank you for contacting Website Designer Ghana",
            Body = emailBody,
            IsHtml = true
        });
    }

    public async Task SendAdminNotificationAsync(string subject, string body)
    {
        await SendEmailAsync(new EmailMessage
        {
            To = _emailSettings.FromEmail,
            ToName = "Admin",
            Subject = subject,
            Body = body,
            IsHtml = true
        });
    }
}
