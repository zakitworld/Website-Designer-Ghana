using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Website_Designer_Ghana.Data;
using Website_Designer_Ghana.Services.Interfaces;
using Website_Designer_Ghana.Services.Models;

namespace Website_Designer_Ghana.Components.Account
{
    internal sealed class IdentityEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IEmailService _emailService;

        public IdentityEmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            _emailService.SendEmailAsync(new EmailMessage
            {
                To = email,
                ToName = user.UserName ?? email,
                Subject = "Confirm your email",
                Body = $"<p>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.</p>",
                IsHtml = true
            });

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            _emailService.SendEmailAsync(new EmailMessage
            {
                To = email,
                ToName = user.UserName ?? email,
                Subject = "Reset your password",
                Body = $"<p>Please reset your password by <a href='{resetLink}'>clicking here</a>.</p>",
                IsHtml = true
            });

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            _emailService.SendEmailAsync(new EmailMessage
            {
                To = email,
                ToName = user.UserName ?? email,
                Subject = "Reset your password",
                Body = $"<p>Please reset your password using the following code: <strong>{resetCode}</strong></p>",
                IsHtml = true
            });
    }
}
