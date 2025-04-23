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

       
        public async Task SendAdminCredentialsEmailAsync(string email, string userName, string password)
        {
          if (string.IsNullOrEmpty(email))
              throw new ArgumentNullException(nameof(email), "Email address cannot be null or empty");
        
          if (string.IsNullOrEmpty(userName))
              throw new ArgumentNullException(nameof(userName), "Username cannot be null or empty");
        
          if (string.IsNullOrEmpty(password))
              throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");
            
            var subject = "Your Administrator Account Credentials";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ padding: 20px; }}
                        .credentials {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 15px 0; }}
                        .footer {{ margin-top: 20px; font-size: 12px; color: #6c757d; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Welcome to the Administration Portal</h2>
                        <p>Dear Administrator,</p>
                        <p>Your administrator account has been created. You can use the following credentials to log in:</p>
                        
                        <div class='credentials'>
                            <p><strong>Username:</strong> {userName}</p>
                            <p><strong>Password:</strong> {password}</p>
                        </div>
                        
                        <p>For security reasons, please change your password after your first login.</p>
                        
                        <p>Thank you,<br>
                        Ra7ala Team</p>
                        
                        <div class='footer'>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
    
               await SendEmailAsync(email, subject, body);
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