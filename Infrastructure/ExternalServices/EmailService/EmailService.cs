using Application.Models;
using Application.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Create email message
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = body };

                // Send email
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string username, string resetToken)
        {
            var subject = "Reset Your Password";
            var body = $@"
                <h1>Reset Your Password</h1>
                <p>Hello {username},</p>
                <p>You have requested to reset your password. Please use the following token to reset your password:</p>
                <p><strong>{resetToken}</strong></p>
                <p>If you did not request a password reset, please ignore this email or contact support if you have concerns.</p>
                <p>Thank you,<br>Ra7ala Team</p>";
            
            await SendEmailAsync(toEmail, subject, body);
        }
    }
}