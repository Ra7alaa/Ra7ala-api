using Application.Models;
using Application.Services.Interfaces;
using Domain.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.ExternalServices.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

       
        public async Task SendAdminCredentialsEmailAsync(string CompanyName,string email, string userName, string password)
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
                            .footer {{ margin-top: 20px; font-size: 12px; color:rgb(234, 241, 135); }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h2>Welcome to the Administration Portal</h2>
                            <p>Dear Administrator,</p>
                            <p>We are delighted to welcome your company, <strong>{CompanyName}</strong>, as part of the Ra7ala platform.</p>
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
                </html>
                ";
    
               await SendEmailAsync(email, subject, body);
            }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private string GetEmailTemplate(string content)
        {
            return $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Ra7ala Transportation</title>
                <style>
                    :root {{
                        --primary: #4CAF50;
                        --primary-dark: #388E3C;
                        --secondary: #2196F3;
                        --text-dark: #333333;
                        --text-light: #757575;
                        --bg-light: #F5F5F5;
                        --white: #FFFFFF;
                        --danger: #F44336;
                    }}
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        line-height: 1.6;
                        color: var(--text-dark);
                        background-color: #f0f0f0;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background: var(--white);
                        border-radius: 8px;
                        overflow: hidden;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        background: linear-gradient(135deg, var(--primary), var(--primary-dark));
                        color: var(--white);
                        padding: 20px;
                        text-align: center;
                    }}
                    .header h1 {{
                        margin: 0;
                        font-size: 24px;
                    }}
                    .logo {{
                        width: 80px;
                        height: auto;
                        margin-bottom: 10px;
                    }}
                    .content {{
                        padding: 30px 40px;
                    }}
                    .credentials {{
                        background-color: #f9f9f9;
                        border-radius: 8px;
                        padding: 20px;
                        margin: 20px 0;
                        border-left: 4px solid var(--primary);
                    }}
                    .button {{
                        display: inline-block;
                        background-color: var(--primary);
                        color: white;
                        text-decoration: none;
                        padding: 12px 25px;
                        border-radius: 4px;
                        margin-top: 15px;
                        font-weight: 600;
                        text-align: center;
                    }}
                    .button:hover {{
                        background-color: var(--primary-dark);
                    }}
                    .footer {{
                        text-align: center;
                        padding: 20px;
                        font-size: 14px;
                        color: var(--text-light);
                        background-color: var(--bg-light);
                        border-top: 1px solid #eeeeee;
                    }}
                    .social {{
                        margin: 15px 0 10px;
                    }}
                    .social a {{
                        display: inline-block;
                        margin: 0 10px;
                        color: var(--text-light);
                        text-decoration: none;
                    }}
                    .warning {{
                        color: var(--danger);
                        font-weight: bold;
                    }}
                    .token {{
                        font-family: monospace;
                        background-color: #f0f0f0;
                        padding: 10px;
                        border-radius: 4px;
                        margin: 15px 0;
                        font-size: 18px;
                        letter-spacing: 1px;
                        text-align: center;
                        border: 1px dashed #ccc;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Ra7ala Transportation</h1>
                    </div>
                    <div class='content'>
                        {content}
                    </div>
                    <div class='footer'>
                        <div>&copy; {DateTime.UtcNow.Year} Ra7ala Transportation. All rights reserved.</div>
                        <div class='social'>
                            <a href='#'>Facebook</a>
                            <a href='#'>Twitter</a>
                            <a href='#'>Instagram</a>
                        </div>
                        <div style='margin-top: 10px;'>If you have any questions, please contact our support team.</div>
                    </div>
                </div>
            </body>
            </html>";
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string username, string resetToken)
        {
            var subject = "Password Reset Request";
            var content = $@"
                <h2>Password Reset</h2>
                <p>Hello {username},</p>
                <p>You've requested to reset your password. Please use the token below:</p>
                <div class='token'>{resetToken}</div>
                <p>If you didn't request a password reset, please ignore this email or contact our support team immediately.</p>
                <p>Thank you,<br>The Ra7ala Team</p>
                <a href='{_emailSettings.FrontendBaseUrl}/reset-password?email={toEmail}&token={Uri.EscapeDataString(resetToken)}' class='button'>Reset Password</a>
            ";

            await SendEmailAsync(toEmail, subject, GetEmailTemplate(content));
        }

        public async Task SendPasswordChangedNotificationAsync(string toEmail, string username)
        {
            var subject = "Password Changed Successfully";
            var content = $@"
                <h2>Password Changed</h2>
                <p>Hello {username},</p>
                <p>We're writing to confirm that your password has been changed successfully.</p>
                <p>If you did not make this change, please contact our support team immediately, as your account may have been compromised.</p>
                <p>Thank you for choosing Ra7ala for your transportation needs.</p>
                <p>Best regards,<br>The Ra7ala Team</p>
                <a href='{_emailSettings.FrontendBaseUrl}/login' class='button'>Login to Your Account</a>
            ";

            await SendEmailAsync(toEmail, subject, GetEmailTemplate(content));
        }

        public async Task SendUserCredientialsEmailAsync(string toEmail, string username, string password)
        {
            var subject = "Welcome to Ra7ala - Your Account Credentials";
            var content = $@"
                <h2>Welcome to Ra7ala!</h2>
                <p>Hello,</p>
                <p>Your account has been created successfully. Below are your login credentials:</p>
                <div class='credentials'>
                    <p><strong>Username/Email:</strong> {username}</p>
                    <p><strong>Password:</strong> {password}</p>
                </div>
                <p class='warning'>Please change your password after your first login for security reasons.</p>
                <p>Thank you for joining our platform!</p>
                <p>Best regards,<br>The Ra7ala Team</p>
                <a href='{_emailSettings.FrontendBaseUrl}/login' class='button'>Login to Your Account</a>
            ";

            await SendEmailAsync(toEmail, subject, GetEmailTemplate(content));
        }
        
        public async Task SendCompanyUserCredientialsEmailAsync(string toEmail, string username, string password, string fullName, string roleName, string companyName)
        {
            var subject = $"Welcome to Ra7ala - Your {roleName} Account at {companyName}";
            var content = $@"
                <h2>Welcome to Ra7ala!</h2>
                <p>Hello {fullName},</p>
                <p>You have been added as a <strong>{roleName}</strong> at <strong>{companyName}</strong>.</p>
                <p>Below are your login credentials:</p>
                <div class='credentials'>
                    <p><strong>Username/Email:</strong> {username}</p>
                    <p><strong>Password:</strong> {password}</p>
                </div>
                <p class='warning'>Please change your password immediately after your first login for security reasons.</p>
                <p>If you have any questions about your role or responsibilities, please contact your company administrator.</p>
                <p>We're excited to have you on board!</p>
                <p>Best regards,<br>The Ra7ala Team</p>
                <a href='{_emailSettings.FrontendBaseUrl}/login' class='button'>Login to Your Account</a>
            ";

            await SendEmailAsync(toEmail, subject, GetEmailTemplate(content));
        }
    }
}