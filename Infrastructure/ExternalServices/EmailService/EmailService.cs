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
            string frontendBaseUrl = _emailSettings.FrontendBaseUrl;
            string resetPasswordUrl = $"{frontendBaseUrl}/reset-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(resetToken)}";
            
            var subject = "Reset Your Ra7ala Password";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Reset Your Password</title>
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap');
                        
                        body {{
                            font-family: 'Poppins', 'Segoe UI', sans-serif;
                            line-height: 1.6;
                            color: #333333;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                        }}
                        .email-container {{
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            background-color: white;
                            border-radius: 8px;
                            overflow: hidden;
                            box-shadow: 0 4px 10px rgba(0,0,0,0.05);
                        }}
                        .email-header {{
                            background-color: #F4C430;
                            padding: 25px 20px;
                            text-align: center;
                            border-radius: 8px 8px 0 0;
                        }}
                        .email-header h1 {{
                            color: #000000;
                            margin: 0;
                            font-size: 26px;
                            font-weight: 600;
                            letter-spacing: 0.5px;
                        }}
                        .email-body {{
                            padding: 35px 30px;
                            background-color: white;
                        }}
                        .email-body h2 {{
                            color: #222222;
                            font-size: 22px;
                            margin-top: 0;
                            font-weight: 600;
                        }}
                        .button-container {{
                            text-align: center;
                            margin: 30px 0;
                        }}
                        .button {{
                            display: inline-block;
                            background-color: #F4C430;
                            color: #000000 !important;
                            padding: 14px 30px;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: 600;
                            letter-spacing: 0.5px;
                            transition: all 0.3s ease;
                            box-shadow: 0 4px 8px rgba(244, 196, 48, 0.3);
                        }}
                        .button:hover {{
                            background-color: #E6B722;
                            transform: translateY(-2px);
                            box-shadow: 0 6px 12px rgba(244, 196, 48, 0.4);
                        }}
                        .email-footer {{
                            text-align: center;
                            margin-top: 30px;
                            padding-top: 20px;
                            border-top: 1px solid #eeeeee;
                            color: #666666;
                            font-size: 12px;
                        }}
                        .signature {{
                            margin-top: 25px;
                            padding-top: 15px;
                            border-top: 1px dashed #eee;
                        }}
                        .warning {{
                            color: #555;
                            font-size: 14px;
                            font-style: italic;
                        }}
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <div class='email-header'>
                            <h1>Ra7ala Transportation</h1>
                        </div>
                        <div class='email-body'>
                            <h2>Password Reset Request</h2>
                            <p>Hello {username},</p>
                            <p>We received a request to reset your password for your Ra7ala account. To complete the reset process, please click on the button below:</p>
                            
                            <div class='button-container'>
                                <a href='{resetPasswordUrl}' class='button'>Reset My Password</a>
                            </div>
                            
                            <p class='warning'>If you didn't request a password reset, please ignore this email or contact our support team if you have concerns.</p>
                            
                            <div class='signature'>
                                <p>Thank you for choosing Ra7ala,<br>The Ra7ala Team</p>
                            </div>
                        </div>
                        <div class='email-footer'>
                            <p>© {DateTime.Now.Year} Ra7ala Transportation. All rights reserved.</p>
                            <p>This email was sent to {toEmail}</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            
            await SendEmailAsync(toEmail, subject, body);
        }
    }
}